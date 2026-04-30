namespace Kawadar.Domain.WalletAndPayments.Enums;

public enum TransactionType
{
  Deposit,
  Withdrawal,
  EscrowHold,
  EscrowRelease,
  EscrowRefund,
  EscrowDeduction,

  SubscriptionCharge,
  EscrowAddition
}