using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetMyPayoutAccounts;

public record GetMyPayoutAccountsQuery : IRequest<Result<List<UserPayoutAccountDto>>>;
