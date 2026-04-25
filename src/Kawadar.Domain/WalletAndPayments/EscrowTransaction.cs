using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Enums;

namespace Kawadar.Domain.WalletAndPayments;


public class EscrowTransaction : AuditableEntity
{
  public Guid ContractId { get; private set; }

  public Guid? ContractMilestoneId { get; private set; }

  public EcrowTransactionType Type { get; private set; }

  public decimal Amount { get; private set; }

  public Guid SenderUserId { get; private set; }
  public Guid ReceiverUserId { get; private set; }

  public string? Note { get; private set; }

  private EscrowTransaction() { }


  private EscrowTransaction(Guid contractId, Guid? contractMilestoneId, EcrowTransactionType type, decimal amount, Guid senderUserId, Guid receiverUserId, string? note = null) : base(Guid.NewGuid())
  {
    ContractId = contractId;
    ContractMilestoneId = contractMilestoneId;
    Type = type;
    Amount = amount;
    SenderUserId = senderUserId;
    ReceiverUserId = receiverUserId;
    Note = note;
  }

  public static Result<EscrowTransaction> Create(Guid contractId, Guid? contractMilestoneId, EcrowTransactionType type, decimal amount, Guid senderUserId, Guid receiverUserId, string? note = null)
  {
    if (amount <= 0)
      return WalletErrors.InvalidAmount;

    var transaction = new EscrowTransaction(contractId, contractMilestoneId, type, amount, senderUserId, receiverUserId, note);
    return transaction;
  }


}