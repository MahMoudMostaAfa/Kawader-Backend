using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Milestones.Commands.SubmitContractMilestone;

public class SubmitContractMilestoneCommandHandler : IRequestHandler<SubmitContractMilestoneCommand, Result<Updated>>
{
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IContractsRepository _contractsRepository;
  private readonly IUnitOfWork _unitOfWork;

  public SubmitContractMilestoneCommandHandler(IUser user, IUsersRepository usersRepository, IContractsRepository contractsRepository, IUnitOfWork unitOfWork)
  {
    _user = user;
    _usersRepository = usersRepository;
    _contractsRepository = contractsRepository;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<Updated>> Handle(SubmitContractMilestoneCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var contractResult = await _contractsRepository.GetContractByIdAsync(request.ContractId, cancellationToken);
    if (contractResult.IsError) return contractResult.Errors;
    var contract = contractResult.Value;

    if (contract.FreelancerId != userProfile.Id)
      return ApplicationErrors.UnauthorizedAccess;

    var submitResult = contract.SubmitMilestone(request.MilestoneId);
    if (submitResult.IsError) return submitResult.Errors;

    await _unitOfWork.SaveChangesAsync(cancellationToken);
    return Result.Updated;
  }
}
