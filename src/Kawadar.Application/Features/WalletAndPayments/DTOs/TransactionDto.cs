using Kawadar.Domain.WalletAndPayments;
using Kawadar.Domain.WalletAndPayments.Enums;

namespace Kawadar.Application.Features.WalletAndPayments.DTOs
{
    public class TransactionDto
    {
        public Guid WalletId { get; set; }
        public TransactionType Type { get; set; }

        public WalletTransactionStatus Status { get; set; } = WalletTransactionStatus.Pending;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";

        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }

        public WalletTransactionReferenceType ReferenceType { get; set; }

        public Guid ReferenceId { get; set; }

        public string? Note { get; set; }
    }
}
