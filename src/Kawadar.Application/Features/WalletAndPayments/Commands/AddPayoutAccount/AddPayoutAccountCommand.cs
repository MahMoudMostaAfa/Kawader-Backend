using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Payouts;
using Kawadar.Domain.WalletAndPayments.Payouts.Enums;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.AddPayoutAccount;

public record AddPayoutAccountCommand(
  PayoutType PayoutType,
  string DisplayName,
  PayoutAccountDetails? AccountDetails,
  bool IsDefault
) : IRequest<Result<Guid>>;
