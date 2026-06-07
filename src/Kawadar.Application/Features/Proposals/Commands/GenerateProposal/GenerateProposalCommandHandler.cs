using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobFiles;
using MediatR;

namespace Kawadar.Application.Features.Proposals.Commands.GenerateProposal;

public class GenerateProposalCommandHandler : IRequestHandler<GenerateProposalCommand, Result<string>>
{
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;

  private readonly IAIService _aIService;
  private readonly IJobsRepository _jobsRepository;

  public GenerateProposalCommandHandler(IUser user, IUsersRepository usersRepository, IAIService aIService, IJobsRepository jobsRepository)
  {
    _user = user;
    _usersRepository = usersRepository;
    _aIService = aIService;
    _jobsRepository = jobsRepository;

  }
  public async Task<Result<string>> Handle(GenerateProposalCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var isProfileEligbleResult = userProfile.IsProfileEgibleToApplyAndPost();
    if(isProfileEligbleResult.IsError) return isProfileEligbleResult.Errors;

    var jobResult = await _jobsRepository.GetJobByIdAsync(request.JobId);

    if(jobResult.IsError) return jobResult.Errors;
    var job = jobResult.Value;

        var prompt = $"""
    You are an expert Arabic freelance proposal writer. Your goal is to write a highly convincing, professional, and tailored project proposal in Arabic based on the provided Job Title and Job Description.

    Rules:
    - The proposal MUST be written entirely in professional, persuasive Arabic (اللغة العربية الفصحى المعاصرة).
    - Adapt the tone to be professional, confident, yet friendly and approachable.
    - Structure the proposal to include:
      1. A strong opening hook that shows understanding of the client's specific problem.
      2. A brief statement of capability (how my skills align perfectly with their needs).
      3. A clear, concise overview of the approach/solution for their specific requirements.
      4. A call to action (CTA) inviting them to a chat to discuss details.
    - Avoid generic templates; make it sound personalized and tailored to the job description.
    - Do NOT include placeholders like "[Your Name]" or "[Price]"; focus only on the body and flow of the message.
    - Keep it concise, engaging, and impactful (100-250 words).

    Job Title:
    {job.Title}

    Job Description:
    {job.Description}

    Proposal Body (in Arabic):
    """;
  
    var proposalResult = await _aIService.GenerateStructuredResponseAsync<string>(prompt, cancellationToken);
        
    if(proposalResult.IsError) return proposalResult.Errors;

        

    return proposalResult.Value;
  }
}