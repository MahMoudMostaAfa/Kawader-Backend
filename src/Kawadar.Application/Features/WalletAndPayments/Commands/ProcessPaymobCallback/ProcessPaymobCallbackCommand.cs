using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.ProcessPaymobCallback;

/// <summary>
/// Command to process a Paymob webhook callback.
/// This is called when Paymob sends a transaction result to our server.
/// </summary>
public record ProcessPaymobCallbackCommand(
  string RawPayload,
  string HmacSignature
) : IRequest<Result<Success>>;
