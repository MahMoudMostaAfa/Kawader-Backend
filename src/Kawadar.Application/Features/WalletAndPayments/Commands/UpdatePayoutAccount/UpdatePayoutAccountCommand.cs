using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Payouts;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.UpdatePayoutAccount;

public record UpdatePayoutAccountCommand(
  Guid AccountId,
  string DisplayName,
  PayoutAccountDetails AccountDetails,
  bool IsDefault
) : IRequest<Result<Updated>>;
