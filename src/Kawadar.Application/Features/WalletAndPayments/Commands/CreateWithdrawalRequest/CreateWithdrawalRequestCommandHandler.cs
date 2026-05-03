using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments;
using Kawadar.Domain.WalletAndPayments.Payouts;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.CreateWithdrawalRequest;

public class CreateWithdrawalRequestCommandHandler : IRequestHandler<CreateWithdrawalRequestCommand, Result<Guid>>
{
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IWalletRepository _walletRepository;
  private readonly IUserPayoutAccountRepository _payoutAccountRepository;
  private readonly IWithdrawalRequestRepository _withdrawalRequestRepository;
  private readonly IUnitOfWork _unitOfWork;

  public CreateWithdrawalRequestCommandHandler(IUser user, IUsersRepository usersRepository,
    IWalletRepository walletRepository, IUserPayoutAccountRepository payoutAccountRepository,
    IWithdrawalRequestRepository withdrawalRequestRepository, IUnitOfWork unitOfWork)
  {
    _user = user;
    _usersRepository = usersRepository;
    _walletRepository = walletRepository;
    _payoutAccountRepository = payoutAccountRepository;
    _withdrawalRequestRepository = withdrawalRequestRepository;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<Guid>> Handle(CreateWithdrawalRequestCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var walletResult = await _walletRepository.GetByUserIdAsync(userProfile.Id, cancellationToken);
    if (walletResult.IsError) return walletResult.Errors;
    var wallet = walletResult.Value;

    if (wallet.Balance < request.Amount) return WalletErrors.InsufficientBalance;

    var payoutAccountResult = await _payoutAccountRepository.GetByIdAsync(request.PayoutAccountId, cancellationToken);
    if (payoutAccountResult.IsError) return payoutAccountResult.Errors;
    var payoutAccount = payoutAccountResult.Value;

    if (payoutAccount.UserId != userProfile.Id) return ApplicationErrors.UnauthorizedAccess;
    if (!payoutAccount.IsActive)
      return Error.Conflict("PayoutAccount.Inactive", "Cannot request withdrawal to an inactive payout account.");

    var withdrawalResult = WithdrawalRequest.Create(wallet.Id, payoutAccount.Id, request.Amount);
    if (withdrawalResult.IsError) return withdrawalResult.Errors;

    var withdrawalRequest = withdrawalResult.Value;
    _withdrawalRequestRepository.Add(withdrawalRequest);

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return withdrawalRequest.Id;
  }
}
