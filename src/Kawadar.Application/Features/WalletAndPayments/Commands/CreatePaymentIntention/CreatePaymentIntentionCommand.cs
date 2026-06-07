using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.CreatePaymentIntention;

public record CreatePaymentIntentionCommand(decimal Amount) : IRequest<Result<CreatePaymentIntentionResult>>;

public record CreatePaymentIntentionResult(
  string IntentionId,
  string ClientSecret,
  decimal Amount,
  string Currency,
  Guid PaymentTransactionId);
