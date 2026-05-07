using AutoMapper;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetAdminWithdrawals;

public class GetAdminWithdrawalsQueryHandler(IWithdrawalRequestRepository withdrawalRequestRepository, IMapper mapper)
  : IRequestHandler<GetAdminWithdrawalsQuery, Result<PaginatedList<WithdrawalRequestDto>>>
{
  public async Task<Result<PaginatedList<WithdrawalRequestDto>>> Handle(GetAdminWithdrawalsQuery request, CancellationToken cancellationToken)
  {
    var withdrawals = await withdrawalRequestRepository.GetAllAsync(
      request.Status,
      request.Page,
      request.PageSize,
      request.SortBy,
      cancellationToken);

    var dtos = withdrawals.Items.Select(mapper.Map<WithdrawalRequestDto>).ToList();

    return new PaginatedList<WithdrawalRequestDto>(dtos, withdrawals.TotalCount, request.Page, request.PageSize);
  }
}
