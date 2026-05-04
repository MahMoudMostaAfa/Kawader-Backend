using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Payouts.Enums;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetAdminWithdrawals;

public record GetAdminWithdrawalsQuery(
  WithdrawalStatus? Status,
  int Page = 1,
  int PageSize = 10,
  string SortBy = "newest") : IRequest<Result<PaginatedList<WithdrawalRequestDto>>>;
