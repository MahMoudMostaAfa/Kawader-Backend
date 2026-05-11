using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Proposals.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Proposals.Queries.GetProposals;

public class GetProposalsQueryHandler : IRequestHandler<GetProposalsQuery, Result<PaginatedList<ProposalSummaryDto>>>
{
  private readonly IUser _user;
  private readonly IProposalsRepository _proposalsRepository;
  private readonly IJobsRepository _jobsRepository;

  private readonly IUsersRepository _usersRepository;

  private readonly IIdentityService _identityService;

  public GetProposalsQueryHandler(IUser user, IProposalsRepository proposalsRepository, IJobsRepository jobsRepository, IUsersRepository usersRepository, IIdentityService identityService)
  {
    _user = user;
    _proposalsRepository = proposalsRepository;
    _jobsRepository = jobsRepository;
    _usersRepository = usersRepository;
    _identityService = identityService;
  }

  public async Task<Result<PaginatedList<ProposalSummaryDto>>> Handle(GetProposalsQuery request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var jobResult = await _jobsRepository.GetJobByIdAsync(request.JobId);
    if (jobResult.IsError) return jobResult.Errors;

    var job = jobResult.Value;
    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);

    if (userProfileResult.IsError) return userProfileResult.Errors;

    var userProfile = userProfileResult.Value;

    if (userProfile.Id != job.PostedById) return ApplicationErrors.UnauthorizedAccess;

    var proposalsResult = await _proposalsRepository.GetProposalsAsync(request.JobId, request.Type, request.Status, request.Page, request.PageSize, request.DatesortBy, request.PriceSortBy, request.EstimatedTimeSortBy);

    var proposals = proposalsResult.Value;

    // freelancer ids to get profiles 
    var freelancerIds = proposals.Items.Select(p => p.FreelancerId).Distinct();
    var profilesResult = await _usersRepository.GetUsersbyIds(freelancerIds);
    var usersIds = profilesResult.Value.Select(up => up.UserId).Distinct();
    var profileByIds = profilesResult.Value.ToDictionary(up => up.Id, up => up);
    var usersResult = await _identityService.GetUsersByIds(usersIds);

    var usersById = usersResult.Value.ToDictionary(u => u.Id, u => u);

    var proposalSummaryDtos = proposals.Items.Select(p =>
    {
      profileByIds.TryGetValue(p.FreelancerId, out var profile);
      usersById.TryGetValue(profile?.UserId ?? string.Empty, out var user);

      return new ProposalSummaryDto
      {

        Id = p.Id,
        CoverLetter = p.CoverLetter,
        FreelancerName = profile?.FullName!,
        FreelancerProfilePictureUrl = profile?.ProfilePictureUrl!,
        FreelancerUsername = user?.UserName!,
        ProposalType = p.ProposalType,
        Status = p.Status
        ,
        ProposedPrice = p.ProposalType switch
        {
          Domain.Proposals.Enums.JobProposalType.OneTime => (decimal)p.Amount!,
          Domain.Proposals.Enums.JobProposalType.Hourly => (decimal)(p.EstimatedHours * p.HourlyRate)!,
          Domain.Proposals.Enums.JobProposalType.MilestoneBased => p.Milestones.Sum(ms => ms.Amount),
          _ => 0m
        },

        CreatedAt = p.CreatedAt,
        TotalMilestones = p.ProposalType == Domain.Proposals.Enums.JobProposalType.MilestoneBased ? p.Milestones.Count() : null,

        EstimatedTimeInDays = p.ProposalType == Domain.Proposals.Enums.JobProposalType.OneTime ? p.EstimatedDays : null,

        EstimatedTimeInHours = p.ProposalType == Domain.Proposals.Enums.JobProposalType.Hourly ? p.EstimatedHours : null,

      }
      ;


    }).ToList();

    return new PaginatedList<ProposalSummaryDto>(proposalSummaryDtos, proposalsResult.Value.TotalCount, proposalsResult.Value.PageNumber, request.PageSize);

  }
}