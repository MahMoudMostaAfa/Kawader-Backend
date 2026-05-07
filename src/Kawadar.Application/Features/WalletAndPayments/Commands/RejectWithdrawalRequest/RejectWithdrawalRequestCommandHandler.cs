using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Payouts.Enums;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.RejectWithdrawalRequest;

public class RejectWithdrawalRequestCommandHandler(
  IUser user,
  IUsersRepository usersRepository,
  IWithdrawalRequestRepository withdrawalRequestRepository,
  IUnitOfWork unitOfWork) : IRequestHandler<RejectWithdrawalRequestCommand, Result<Updated>>
{
  public async Task<Result<Updated>> Handle(RejectWithdrawalRequestCommand request, CancellationToken cancellationToken)
  {
    var userId = user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var adminProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
    if (adminProfileResult.IsError) return adminProfileResult.Errors;

    var withdrawalResult = await withdrawalRequestRepository.GetByIdAsync(request.WithdrawalRequestId, cancellationToken);
    if (withdrawalResult.IsError) return withdrawalResult.Errors;
    var withdrawal = withdrawalResult.Value;

    if (withdrawal.Status != WithdrawalStatus.Pending)
      return Error.Conflict("WithdrawalRequest.InvalidStatus", "Only pending withdrawal requests can be rejected.");

    withdrawal.MarkAsFailed(request.Reason, adminProfileResult.Value.Id);

    await unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Updated;
  }
}
