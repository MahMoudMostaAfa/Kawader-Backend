using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetAllTransactions
{
    public class GetAllTransactionsQueryHandler(IUser user, IWalletRepository walletRepository,
        IMapper mapper) : IRequestHandler<GetAllTransactionsQuery, Result<PaginatedList<TransactionDto>>>
    {
        public async Task<Result<PaginatedList<TransactionDto>>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var transactions = await walletRepository.GetAllTransactions(
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
