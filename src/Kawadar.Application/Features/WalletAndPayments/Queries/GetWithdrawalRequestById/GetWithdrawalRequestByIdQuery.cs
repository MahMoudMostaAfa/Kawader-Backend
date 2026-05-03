using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetWithdrawalRequestById;

public record GetWithdrawalRequestByIdQuery(Guid WithdrawalRequestId) : IRequest<Result<WithdrawalRequestDto>>;
