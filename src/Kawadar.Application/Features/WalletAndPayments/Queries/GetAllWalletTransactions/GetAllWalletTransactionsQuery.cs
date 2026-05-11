using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Enums;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetAllWalletTransactions
{
    public record GetAllWalletTransactionsQuery(Guid WalletId, TransactionType? type, WalletTransactionStatus? status, WalletTransactionReferenceType? referenceType,
        int page, int pageSize, string sortBy) : IRequest<Result<PaginatedList<TransactionDto>>>;
}
