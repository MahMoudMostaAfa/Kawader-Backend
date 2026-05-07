using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.WalletAndPayments;
using Kawadar.Domain.WalletAndPayments.Enums;
using Microsoft.Extensions.Logging;

namespace Kawadar.Infrastructure.Services.BackgroundJobs;

public class EscrowReleaseJob
{

  private readonly IUnitOfWork _unitOfWork;
  private readonly ILogger<EscrowReleaseJob> _logger;
  private readonly IWalletRepository _walletRepository;
  public EscrowReleaseJob(IUnitOfWork unitOfWork, ILogger<EscrowReleaseJob> logger, IWalletRepository walletRepository)
  {
    _unitOfWork = unitOfWork;
    _logger = logger;
    _walletRepository = walletRepository;

  }
  public async Task ExecuteAsync(Guid escrowTransactionId, CancellationToken cancellationToken)
  {

    var escrowTransactionResult = await _walletRepository.GetEscrowTransactionById(escrowTransactionId, cancellationToken);
    if (escrowTransactionResult.IsError)
    {
      _logger.LogError("Failed to retrieve escrow transaction with id {EscrowTransactionId}.", escrowTransactionId);
      // throw error to trigger retry mechanism
      throw new InvalidOperationException($"Failed to retrieve escrow transaction with id {escrowTransactionId}.");
    }
    var transaction = escrowTransactionResult.Value;

    var walletResult = await _walletRepository.GetByUserIdAsync(transaction.ReceiverUserId);
    if (walletResult.IsError)
    {
      _logger.LogError("Failed to retrieve wallet for user {UserId}.", transaction.ReceiverUserId);
      // throw error to trigger retry mechanism
      throw new InvalidOperationException($"Failed to retrieve wallet for user {transaction.ReceiverUserId}.");
    }
    var wallet = walletResult.Value;
    wallet.AddTransaction(transaction.Amount, TransactionType
    .EscrowRelease, WalletTransactionReferenceType.Contract, transaction.Id, null, WalletTransactionStatus.Completed);

    await _unitOfWork.SaveChangesAsync(cancellationToken);

  }
}
