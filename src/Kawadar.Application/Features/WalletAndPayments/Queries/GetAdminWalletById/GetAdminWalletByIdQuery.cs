using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetAdminWalletById;

public record GetAdminWalletByIdQuery(Guid WalletId) : IRequest<Result<AdminWalletDto>>;
