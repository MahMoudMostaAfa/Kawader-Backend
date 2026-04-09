using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Proposals.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Application.Features.Proposals.Commands.UpdateProposalStatus;


public class UpdateProposalStatusCommandHandler :
IRequestHandler<UpdateProposalStatusCommand, Result<Updated>>
{
  private readonly IUnitOfWork _unitOfWork;

  private readonly IProposalsRepository _proposalsRepository;

  private readonly IUser _user;
  private readonly IJobsRepository _jobsRepository;

  private readonly IUsersRepository _usersRepository;
  private readonly ILogger<UpdateProposalStatusCommandHandler> _logger;
  public UpdateProposalStatusCommandHandler(IUnitOfWork unitOfWork, IProposalsRepository proposalsRepository, IUser user, IJobsRepository jobsRepository, IUsersRepository usersRepository, ILogger<UpdateProposalStatusCommandHandler> logger)
  {
    _unitOfWork = unitOfWork;
    _proposalsRepository = proposalsRepository;
    _user = user;
    _jobsRepository = jobsRepository;
    _usersRepository = usersRepository;
    _logger = logger;

  }
  public async Task<Result<Updated>> Handle(UpdateProposalStatusCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;

    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;
    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var proposalResult = await _proposalsRepository.GetByIdAsync(request.ProposalId);
    if (proposalResult.IsError) return proposalResult.Errors;
    var proposal = proposalResult.Value;
    var jobResult = await _jobsRepository.GetJobByIdAsync(proposal.JobId);
    if (jobResult.IsError) return jobResult.Errors;
    var job = jobResult.Value;

    if (job.PostedById != userProfile.Id) return ApplicationErrors.UnauthorizedAccess;

    // if the proposal is already withdrawn or excluded, we can not update its status
    if (proposal.Status == JobProposalStatus.Withdrawn || proposal.Status == JobProposalStatus.Excluded) return Error.NotFound();


    var proposalUpdateStatusResult = proposal.UpdateState(request.NewProposalStatus);

    if (proposalUpdateStatusResult.IsError) return proposalUpdateStatusResult.Errors;

    await _unitOfWork.SaveChangesAsync(cancellationToken);
    _logger.LogInformation("Proposal with id {ProposalId} has been updated to status {NewStatus} by user {UserId}", request.ProposalId, request.NewProposalStatus, userId);

    return Result.Updated;

  }
}