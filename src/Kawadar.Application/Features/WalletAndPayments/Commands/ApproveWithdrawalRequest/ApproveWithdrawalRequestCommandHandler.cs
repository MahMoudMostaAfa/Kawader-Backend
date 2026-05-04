using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Enums;
using Kawadar.Domain.WalletAndPayments.Payouts.Enums;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.ApproveWithdrawalRequest;

public class ApproveWithdrawalRequestCommandHandler(
  IUser user,
  IUsersRepository usersRepository,
  IWalletRepository walletRepository,
  IWithdrawalRequestRepository withdrawalRequestRepository,
  IUnitOfWork unitOfWork) : IRequestHandler<ApproveWithdrawalRequestCommand, Result<Updated>>
{
  public async Task<Result<Updated>> Handle(ApproveWithdrawalRequestCommand request, CancellationToken cancellationToken)
  {
    var userId = user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var adminProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
    if (adminProfileResult.IsError) return adminProfileResult.Errors;

    var withdrawalResult = await withdrawalRequestRepository.GetByIdAsync(request.WithdrawalRequestId, cancellationToken);
    if (withdrawalResult.IsError) return withdrawalResult.Errors;
    var withdrawal = withdrawalResult.Value;

    if (withdrawal.Status != WithdrawalStatus.Pending)
      return Error.Conflict("WithdrawalRequest.InvalidStatus", "Only pending withdrawal requests can be approved.");

    var walletResult = await walletRepository.GetByIdAsync(withdrawal.WalletId, cancellationToken);
    if (walletResult.IsError) return walletResult.Errors;
    var wallet = walletResult.Value;

    var transactionResult = wallet.AddTransaction(
      withdrawal.Amount,
      TransactionType.Withdrawal,
      WalletTransactionReferenceType.Manual,
      withdrawal.Id,
      "Admin withdrawal payout");
    if (transactionResult.IsError) return transactionResult.Errors;

    var transaction = transactionResult.Value;
    walletRepository.AddWalletTransaction(transaction);

    withdrawal.MarkAsCompleted(transaction.Id, adminProfileResult.Value.Id);

    await unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Updated;
  }
}
