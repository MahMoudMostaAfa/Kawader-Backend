using AutoMapper;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetAdminWalletById;

public class GetAdminWalletByIdQueryHandler(IWalletRepository walletRepository, IMapper mapper)
  : IRequestHandler<GetAdminWalletByIdQuery, Result<AdminWalletDto>>
{
  public async Task<Result<AdminWalletDto>> Handle(GetAdminWalletByIdQuery request, CancellationToken cancellationToken)
  {
    var walletResult = await walletRepository.GetByIdAsync(request.WalletId, cancellationToken);
    if (walletResult.IsError) return walletResult.Errors;

    var dto = mapper.Map<AdminWalletDto>(walletResult.Value);

    dto.TotalProfit = await walletRepository.GetTotalProfitByWalletId(request.WalletId, cancellationToken);

    return dto;
  }
}
