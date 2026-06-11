using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Payments;
using Kawadar.Domain.WalletAndPayments.Payments.Enums;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.CreatePaymentIntention;

public class CreatePaymentIntentionCommandHandler
  : IRequestHandler<CreatePaymentIntentionCommand, Result<CreatePaymentIntentionResult>>
{
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IIdentityService _identityService;
  private readonly IWalletRepository _walletRepository;
  private readonly IPaymentRepository _paymentRepository;
  private readonly IPaymobService _paymobService;
  private readonly IUnitOfWork _unitOfWork;
  private readonly IIdentityService _identityService;

  public CreatePaymentIntentionCommandHandler(
    IUser user,
    IUsersRepository usersRepository,
    IWalletRepository walletRepository,
    IPaymentRepository paymentRepository,
    IPaymobService paymobService,
    IUnitOfWork unitOfWork,
    IIdentityService identityService)
  {
    _user = user;
    _usersRepository = usersRepository;
    _walletRepository = walletRepository;
    _paymentRepository = paymentRepository;
    _paymobService = paymobService;
    _unitOfWork = unitOfWork;
    _identityService = identityService;
  }

  public async Task<Result<CreatePaymentIntentionResult>> Handle(
    CreatePaymentIntentionCommand request,
    CancellationToken cancellationToken)
  {
    // 1. Authenticate user
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    // 2. Get user's wallet
    var walletResult = await _walletRepository.GetByUserIdAsync(userProfile.Id, cancellationToken);
    if (walletResult.IsError) return walletResult.Errors;
    var wallet = walletResult.Value;

        var userIdentityResult = await _identityService.GetUserByIdAsync(userProfile.UserId);
        if (userIdentityResult.IsError) return userIdentityResult.Errors;
        var userDto = userIdentityResult.Value;

    // 3. Create billing data from user profile
    var billingData = new PaymobBillingData(
      FirstName: userProfile.FirstName ?? "NA",
      LastName: userProfile.LastName ?? "NA",
      Email: userDto.Email ?? "NA",
      PhoneNumber: userProfile.PhoneNumber ?? "NA"
    );

    // 4. Create payment intention on Paymob
    var intentionResult = await _paymobService.CreatePaymentIntentionAsync(
      amount: request.Amount,
      currency: "EGP",
      paymentMethodIds: ["4892084"], // Card integration ID from Paymob dashboard
      billingData: billingData,
      internalOrderId: null,
      ct: cancellationToken);

    if (intentionResult.IsError) return intentionResult.Errors;
    var intention = intentionResult.Value;

    // 5. Create local PaymentTransaction record (Pending)
    var paymentTxResult = PaymentTransaction.Create(
      userId: userProfile.Id,
      walletId: wallet.Id,
      amount: request.Amount,
      gateway: PaymentGateway.Paymob,
      method: PaymentMethod.Card,
      gatewayTransactionId: intention.IntentionId,
      gatewayOrderId: intention.IntentionId);

    if (paymentTxResult.IsError) return paymentTxResult.Errors;
    var paymentTx = paymentTxResult.Value;

    _paymentRepository.Add(paymentTx);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    // 6. Return client secret + internal payment ID to the frontend
    return new CreatePaymentIntentionResult(
      IntentionId: intention.IntentionId,
      ClientSecret: intention.ClientSecret,
      Amount: request.Amount,
      Currency: "EGP",
      PaymentTransactionId: paymentTx.Id);
  }
}
