using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Payouts.Enums;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetMyWithdrawalRequests;

public record GetMyWithdrawalRequestsQuery(WithdrawalStatus? Status) : IRequest<Result<List<WithdrawalRequestDto>>>;
