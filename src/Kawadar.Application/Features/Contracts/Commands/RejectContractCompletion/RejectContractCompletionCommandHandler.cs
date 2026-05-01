using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Contracts.Events;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Commands.RejectContractCompletion;

public class RejectContractCompletionCommandHandler :
IRequestHandler<RejectContractCompletionCommand,
Result<Updated>>
{
  private readonly IContractsRepository _contractsRepository;
  private readonly IUnitOfWork _unitOfWork;
  private readonly IUsersRepository _usersRepository;
  private readonly IUser _user;


  public RejectContractCompletionCommandHandler(IContractsRepository contractsRepository, IUnitOfWork unitOfWork, IUsersRepository usersRepository, IUser user)
  {
    _contractsRepository = contractsRepository;
    _unitOfWork = unitOfWork;
    _usersRepository = usersRepository;
    _user = user;


  }
  public async Task<Result<Updated>> Handle(RejectContractCompletionCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var contractResult = await _contractsRepository.GetContractByIdAsync(request.ContractId);
    if (contractResult.IsError) return contractResult.Errors;
    var contract = contractResult.Value;

    if (contract.ClientId != userProfile.Id)
      return ApplicationErrors.UnauthorizedAccess;

    var rejectCompletionResult = contract.RejectCompletion(request.Reason);
    if (rejectCompletionResult.IsError) return rejectCompletionResult.Errors;


    var userProfile2Result = await _usersRepository.GetUserProfileByIdAsync(contract.FreelancerId);
    if (userProfile2Result.IsError) return userProfile2Result.Errors;
    var userProfile2 = userProfile2Result.Value;

    contract.AddDomainEvent(new RejectCompletionContractEvent(contract.Id, userProfile2.Id, userProfile2.UserId, request.Reason));


    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Updated;
  }
}