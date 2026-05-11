
using Kawadar.Domain.WalletAndPayments.Enums;

namespace Kawadar.Application.Features.Admins.Dtos
{
    public class FinancialStatisticsDto
    {
        public Dictionary<string, decimal> TotalMoneyTransfered { get; set; } = default!;
        public decimal TotalProfit { get; set; }
        public Dictionary<WalletTransactionStatus, int> TransactionStatusDistribution { get; set; } = default!;
    }
}
