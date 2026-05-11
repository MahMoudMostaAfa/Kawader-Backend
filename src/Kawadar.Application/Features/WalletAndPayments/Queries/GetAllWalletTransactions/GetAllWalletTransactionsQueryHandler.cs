using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetAllWalletTransactions
{
    public class GetAllWalletTransactionsQueryHandler(IUser user,
        IWalletRepository walletRepository, IMapper mapper) : IRequestHandler<GetAllWalletTransactionsQuery, Result<PaginatedList<TransactionDto>>>
    {
        public async Task<Result<PaginatedList<TransactionDto>>> Handle(GetAllWalletTransactionsQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var walletResult = await walletRepository.GetByIdAsync(request.WalletId);
            if (walletResult.IsError) return walletResult.Errors;

            var transactions = await walletRepository.GetAllTransactionsByWalletId(
                request.WalletId,
                request.type,
                request.status,
                request.referenceType,
                request.page,
                request.pageSize,
                request.sortBy,
                cancellationToken);

            var transactionDtos = transactions.Items.Select(t => mapper.Map<TransactionDto>(t)).ToList();

            return new PaginatedList<TransactionDto>(transactionDtos, transactions.TotalCount, request.page, request.pageSize);
        }
    }
}
