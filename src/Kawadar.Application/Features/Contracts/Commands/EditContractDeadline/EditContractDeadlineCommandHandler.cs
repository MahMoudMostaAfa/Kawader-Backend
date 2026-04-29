using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Commands.EditContractDeadline;

public class EditContractDeadlineCommandHandler : IRequestHandler<EditContractDeadlineCommand, Result<Updated>>
{
  private readonly IUser _user;
  private readonly IContractsRepository _contractsRepository;
  private readonly IUsersRepository _usersRepository;
  private readonly IUnitOfWork _unitOfWork;

  public EditContractDeadlineCommandHandler(IUser user, IContractsRepository contractsRepository, IUsersRepository usersRepository, IUnitOfWork unitOfWork)
  {
    _user = user;
    _contractsRepository = contractsRepository;
    _usersRepository = usersRepository;
    _unitOfWork = unitOfWork;

  }
  public async Task<Result<Updated>> Handle(EditContractDeadlineCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;

    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;
    var contractResult = await _contractsRepository.GetContractByIdAsync(request.ContractId, cancellationToken);
    if (contractResult.IsError) return contractResult.Errors;
    var contract = contractResult.Value;
    if (contract.ClientId != userProfile.Id) return ApplicationErrors.UnauthorizedAccess;

    var updateResult = contract.ChangeDeadline(request.NewDeadline);
    if (updateResult.IsError) return updateResult.Errors;



    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Updated;





  }
}