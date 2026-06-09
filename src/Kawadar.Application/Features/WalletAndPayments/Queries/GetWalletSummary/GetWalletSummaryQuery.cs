using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Enums;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetWalletSummary;

public record GetWalletSummaryQuery(
    TransactionType? Type,
    WalletTransactionStatus? Status,
    WalletTransactionReferenceType? ReferenceType,
    int Page,
    int PageSize,
    string SortBy) : IRequest<Result<PaginatedList<TransactionDto>>>;
