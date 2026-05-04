using AutoMapper;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetAdminWallets;

public class GetAdminWalletsQueryHandler(IWalletRepository walletRepository, IMapper mapper)
  : IRequestHandler<GetAdminWalletsQuery, Result<PaginatedList<AdminWalletDto>>>
{
  public async Task<Result<PaginatedList<AdminWalletDto>>> Handle(GetAdminWalletsQuery request, CancellationToken cancellationToken)
  {
    var wallets = await walletRepository.GetWalletsAsync(
      request.UserId,
      request.IsActive,
      request.MinBalance,
      request.MaxBalance,
      request.Page,
      request.PageSize,
      request.SortBy,
      cancellationToken);

    var dtos = wallets.Items.Select(mapper.Map<AdminWalletDto>).ToList();

    return new PaginatedList<AdminWalletDto>(dtos, wallets.TotalCount, request.Page, request.PageSize);
  }
}
