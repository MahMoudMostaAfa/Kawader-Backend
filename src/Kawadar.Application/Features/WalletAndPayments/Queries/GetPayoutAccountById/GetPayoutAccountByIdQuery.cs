using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetPayoutAccountById;

public record GetPayoutAccountByIdQuery(Guid AccountId) : IRequest<Result<UserPayoutAccountDto>>;
