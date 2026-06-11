using System.Security.Cryptography.X509Certificates;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Proposals.Commands.DeleteProposal;


public class DeleteProposalCommandHandler : IRequestHandler<DeleteProposalCommand, Result<Deleted>>
{

  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IProposalsRepository _proposalsRepository;

  private readonly IUnitOfWork _unitOfWork;

  public DeleteProposalCommandHandler(IUser user, IUsersRepository usersRepository, IProposalsRepository proposalsRepository, IUnitOfWork unitOfWork)
  {

    _user = user;
    _proposalsRepository = proposalsRepository;

    _usersRepository = usersRepository;
    _unitOfWork = unitOfWork;
  }
  public async Task<Result<Deleted>> Handle(DeleteProposalCommand request, CancellationToken cancellationToken)
  {

    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;



    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;


    var proposalResult = await _proposalsRepository.GetByIdAsync(request.ProposalId);
    if (proposalResult.IsError) return proposalResult.Errors;




    var proposal = proposalResult.Value;
        if (proposal.Status == Domain.Proposals.Enums.JobProposalStatus.Withdrawn) return Error.Validation("This Proposal is already withdrawn.");

    if (proposal.FreelancerId != userProfile.Id) return ApplicationErrors.UnauthorizedAccess;


    proposal.UpdateState(Domain.Proposals.Enums.JobProposalStatus.Withdrawn);

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Deleted;
  }
}