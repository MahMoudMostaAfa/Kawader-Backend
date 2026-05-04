using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetAdminWallets;

public record GetAdminWalletsQuery(
  Guid? UserId,
  bool? IsActive,
  decimal? MinBalance,
  decimal? MaxBalance,
  int Page = 1,
  int PageSize = 10,
  string SortBy = "newest") : IRequest<Result<PaginatedList<AdminWalletDto>>>;
