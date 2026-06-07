using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.ProfileManagment.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Application.Features.ProfileManagment.Commands.UploadAndAutoFill;

public class UploadAndAutoFillCommandHandler : IRequestHandler<UploadAndAutoFillCommand, Result<ProfileAutoFillDto>>
{
  private readonly IUser _user;
  private readonly IAIService _aiService;
  private readonly IUsersRepository _usersRepository;
  private readonly IPDFService _pdfService;
  private readonly ILogger<UploadAndAutoFillCommandHandler> _logger;
  public UploadAndAutoFillCommandHandler(IUser user, IAIService aiService, IUsersRepository usersRepository, IPDFService pdfService
  , ILogger<UploadAndAutoFillCommandHandler> logger)
  {
    _user = user;
    _aiService = aiService;
    _usersRepository = usersRepository;
    _pdfService = pdfService;
    _logger = logger;
  }
  public async Task<Result<ProfileAutoFillDto>> Handle(UploadAndAutoFillCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;

    var textResult = _pdfService.ExtractTextFromPdfAsync(request.File.OpenReadStream(), cancellationToken);
    if (textResult.IsError) return textResult.Errors;
    var extractedText = textResult.Value;

    _logger.LogInformation("Extracted text from PDF for user {UserId}: {ExtractedText}", userId, extractedText);

    var prompt = $"""
            You are an expert HR recruitment assistant specialized in parsing Arabic and English resumes.
            Analyze the following resume content. Extract the core information and map it into the JSON structure requested below.
            The text might have minor text reversal or alignment bugs caused by PDF parsing—fix these intelligently based on context.

            Rules:
            - Return ONLY raw valid JSON matching the format. Do NOT wrap the response in markdown blocks like ```json.
            - Extract skills into a clean array of strings.
            - If a field is missing, set its value to null or 0.
            - for experienceyears ,map LessThanOneYear to 0, OneToThreeYears to 1,ThreeToFiveYears to 2, FiveToTenYears to 3, MoreThanTenYears to 4.

            Resume Content:
            ---
            {extractedText}
            ---
            """;
    var aiResponseResult = await _aiService.GenerateStructuredResponseAsync<ProfileAutoFillDto>(prompt, cancellationToken);
    if (aiResponseResult.IsError) return aiResponseResult.Errors;
    var profileAutoFillDto = aiResponseResult.Value;

    return profileAutoFillDto;


  }
}
