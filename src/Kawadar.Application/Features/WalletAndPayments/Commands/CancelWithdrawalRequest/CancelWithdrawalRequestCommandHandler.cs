using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Payouts.Enums;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.CancelWithdrawalRequest;

public class CancelWithdrawalRequestCommandHandler : IRequestHandler<CancelWithdrawalRequestCommand, Result<Deleted>>
{
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IWalletRepository _walletRepository;
  private readonly IWithdrawalRequestRepository _withdrawalRequestRepository;
  private readonly IUnitOfWork _unitOfWork;

  public CancelWithdrawalRequestCommandHandler(IUser user, IUsersRepository usersRepository,
    IWalletRepository walletRepository, IWithdrawalRequestRepository withdrawalRequestRepository,
    IUnitOfWork unitOfWork)
  {
    _user = user;
    _usersRepository = usersRepository;
    _walletRepository = walletRepository;
    _withdrawalRequestRepository = withdrawalRequestRepository;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<Deleted>> Handle(CancelWithdrawalRequestCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var walletResult = await _walletRepository.GetByUserIdAsync(userProfile.Id, cancellationToken);
    if (walletResult.IsError) return walletResult.Errors;
    var wallet = walletResult.Value;

    var withdrawalResult = await _withdrawalRequestRepository.GetByIdAsync(request.WithdrawalRequestId, cancellationToken);
    if (withdrawalResult.IsError) return withdrawalResult.Errors;
    var withdrawal = withdrawalResult.Value;

    if (withdrawal.WalletId != wallet.Id) return ApplicationErrors.UnauthorizedAccess;

    if (withdrawal.Status != WithdrawalStatus.Pending)
      return Error.Conflict("WithdrawalRequest.InvalidStatus", "Only pending withdrawal requests can be cancelled.");

    withdrawal.ChangeStatus(WithdrawalStatus.Cancelled);

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Deleted;
  }
}
