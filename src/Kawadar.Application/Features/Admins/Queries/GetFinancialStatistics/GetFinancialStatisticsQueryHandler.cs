using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Admins.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Admins.Queries.GetFinancialStatistics
{
    public class GetFinancialStatisticsQueryHandler(IUser user, IWalletRepository walletRepository) : IRequestHandler<GetFinancialStatisticsQuery, Result<FinancialStatisticsDto>>
    {
        public async Task<Result<FinancialStatisticsDto>> Handle(GetFinancialStatisticsQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var totalProfit = await walletRepository.GetTotalProfit();
            var distributionBasedOnCurrency = await walletRepository.GetMoneyTransactionDistributionBasedOnCurrency();
            var transactionDistributionBasedOnStatus = await walletRepository.GetTransactionStatusDistribution();
            var totalBalance = await walletRepository.GetTotalBalance();
            var totalEscrow = await walletRepository.GetTotalEscrow();

            return new FinancialStatisticsDto
            {
                TotalProfit = totalProfit,
                TransactionStatusDistribution = transactionDistributionBasedOnStatus,
                TotalMoneyTransfered = distributionBasedOnCurrency,
                totalBalance = totalBalance,
                totalEscrow = totalEscrow
            };
        }
    }
}
