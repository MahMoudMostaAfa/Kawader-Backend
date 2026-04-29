using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Contracts.Enums;
using Kawadar.Domain.WalletAndPayments;
using Kawadar.Domain.WalletAndPayments.Enums;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Commands.CancelContract;


public class CancelContractCommnandHandler : IRequestHandler<CancelContractCommand, Result<Updated>>
{
  private readonly IContractsRepository _contractsRepository;
  private readonly IUnitOfWork _unitOfWork;
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IWalletRepository _walletRepository;

  public CancelContractCommnandHandler(IContractsRepository contractsRepository, IUnitOfWork unitOfWork, IUser user, IUsersRepository usersRepository, IWalletRepository walletRepository)
  {
    _contractsRepository = contractsRepository;
    _unitOfWork = unitOfWork;
    _user = user;
    _usersRepository = usersRepository;
    _walletRepository = walletRepository;
  }



  public async Task<Result<Updated>> Handle(CancelContractCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;


    var contractResult = await _contractsRepository.GetContractByIdAsync(request.ContractId, cancellationToken);
    if (contractResult.IsError) return contractResult.Errors;
    var contract = contractResult.Value;
    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;
    if (contract.ClientId != userProfile.Id) return ApplicationErrors.UnauthorizedAccess;
    if (DateTime.UtcNow - contract.CreatedAt > TimeSpan.FromHours(24))
      return Error.Failure("Contracts.Cancels", "Contract can only be cancelled within 24 hours of creation.");

    if (contract.Type != Domain.Contracts.Enums.ContractType.OneTime)
      return Error.Failure("Contracts.Cancels", "Only one-time contracts can be cancelled.");



    /// get escrow transaction 
    /// if exists return the amount to client wallet and update transaction status to cancelled

    var escrowTransactionResult = await _walletRepository.GetEscrowTransactionByContractId(contract.Id, cancellationToken);
    if (escrowTransactionResult.IsError) return escrowTransactionResult.Errors;
    var escrowTransaction = escrowTransactionResult.Value;
    if (escrowTransaction.SenderUserId != userProfile.Id || escrowTransaction.Type != EcrowTransactionType.Hold) return ApplicationErrors.UnauthorizedAccess;



    var cancelResult = contract.ChangeStatus(ContractStatus.Canceled);
    if (cancelResult.IsError) return cancelResult.Errors;

    // create escrow transaction to return the amount to client wallet

    var escrowReturnTransactionResult = EscrowTransaction.Create(contract.Id, null, EcrowTransactionType.Refund, contract.OneTimeFixedPrice ?? 0, escrowTransaction.SenderUserId, escrowTransaction.ReceiverUserId, null);

    var escrowReturnTransaction = escrowReturnTransactionResult.Value;
    _walletRepository.AddEscrowTransaction(escrowReturnTransaction);

    var walletResult = await _walletRepository.GetByUserIdAsync(userProfile.Id, cancellationToken);
    if (walletResult.IsError) return walletResult.Errors;
    var wallet = walletResult.Value;
    var walletTransactionResult = wallet.AddTransaction(escrowTransaction.Amount, TransactionType.EscrowRefund, WalletTransactionReferenceType.Contract, contract.Id, null, WalletTransactionStatus.Completed);

    if (walletTransactionResult.IsError) return walletTransactionResult.Errors;



    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Updated;

  }
}