using System.Text.Json.Serialization;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.ProfileManagment.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.ProfileManagment.Queries.GetFreelancersByAi;


public class GetFreelancersByAiQueryHandler : IRequestHandler<GetFreelancersByAiQuery, Result<IEnumerable<BriefFreelancerWithStrengthDto>>>
{
  private readonly IUsersRepository _usersRepository;
  private readonly IFreelancerVectorStore _freelancerVectorStore;
  private readonly IIdentityService _identityService;
  private readonly IAIChatService _aiChatService;
  private readonly IUser _user;

  private const string SystemPrompt = """
    You are a freelancer matching assistant.

    User needs: "{0}"

    Freelancers:
    {1}

    Return ONLY a JSON array. No explanation, no markdown. Example format:
    [
      {{
        "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
        "strength": "Comment on why this freelancer is a good match for the user's needs in English. "
      }}
    ]

    Rules:
    - Use the exact ID values provided in the Freelancers list above (they are GUIDs)
    - Only include freelancers RELEVANT to the request
    - strength should be a concise comment on why this freelancer is a good match for the user's needs, based on their skills, experience, and other relevant information.
    """;

  public GetFreelancersByAiQueryHandler(IUser user, IAIChatService aiChatService, IIdentityService identityService, IFreelancerVectorStore freelancerVectorStore, IUsersRepository usersRepository
  )
  {
    _user = user;
    _aiChatService = aiChatService;
    _identityService = identityService;
    _freelancerVectorStore = freelancerVectorStore;
    _usersRepository = usersRepository;



  }
  public async Task<Result<IEnumerable<BriefFreelancerWithStrengthDto>>> Handle(GetFreelancersByAiQuery request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var freelancersResult = await _freelancerVectorStore.SearchFreelancersIdsAsync(request.Query, 10);
    if (freelancersResult.IsError) return freelancersResult.Errors;
    var freelancers = freelancersResult.Value?
      .Where(f => f.Id != userProfile.Id)
      .ToList() ?? [];

    if (freelancers.Count == 0)
      return new List<BriefFreelancerWithStrengthDto>();

    var identitiesIDs = freelancers.Select(f => f.UserId);

    var identitiesResult = await _identityService.GetUsersByIds(identitiesIDs);
    var identities = identitiesResult.Value;

    var context = string.Join("\n\n", freelancers.Select(f =>
      $"ID: {f.Id}\n" +
      $"Skills: {(f.Skills != null ? string.Join(", ", f.Skills) : "Not specified")}\n" +
      $"Experience: {f.ExperienceYear.ToString()} years\n" +
      $"Average Rating: {(f.Reviews != null && f.Reviews.Count > 0 ? f.Reviews.Average(r => r.Rating) : 0)}\n" +
      $"Bio: {f.Bio ?? "No bio available."}\n" +
      $"Full Name: {f.FullName} "
      + $"Specialization: {f.Specialization?.Name ?? "Not specified"}"
      + $"Title: {f.Title ?? "Not specified"}"
  ));
    var aiResponseResult = await _aiChatService.ChatAsync<List<AiResponseDto>>(
      "",
      string.Format(SystemPrompt, request.Query, context),
      cancellationToken);

    if (aiResponseResult.IsError) return aiResponseResult.Errors;

    var aiResponse = aiResponseResult.Value ?? [];
    var response = aiResponse
      .Where(r => Guid.TryParse(r.Id, out _))
      .ToDictionary(r => Guid.Parse(r.Id), r => r.Strength);

    var finalResult = freelancers.Join(
      identities,
      f => f.UserId,
      i => i.Id,
      (f, i) => new BriefFreelancerWithStrengthDto
      {
        Id = f.Id,
        fullName = f.FullName,
        PhotoUrl = f.ProfilePictureUrl ?? "",
        Strength = [response.TryGetValue(f.Id, out var strength) ? strength : string.Empty],
        IsOnline = f.IsOnline,
        AverageRating = f.Reviews != null && f.Reviews.Count > 0 ? f.Reviews.Average(r => r.Rating) : 0
        ,
        UserName = i.UserName

      });



    return finalResult.ToList();

  }


  private class AiResponseDto
  {
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    [JsonPropertyName("strength")]
    public string Strength { get; set; } = string.Empty;
  }
}