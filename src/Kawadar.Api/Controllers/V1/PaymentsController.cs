using Kawadar.Application.Features.WalletAndPayments.Commands.CreatePaymentIntention;
using Kawadar.Application.Features.WalletAndPayments.Commands.ProcessPaymobCallback;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1;


[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/payments")]
public class PaymentsController : ApiController
{
  private readonly ISender _sender;

  public PaymentsController(ISender sender)
  {
    _sender = sender;
  }


  /// <summary>
  /// Creates a Paymob payment intention for wallet deposit.
  /// Returns the client_secret needed by the frontend to launch the Paymob checkout.
  /// </summary>
  [Authorize]
  [HttpPost("intention")]
  [ProducesResponseType(typeof(CreatePaymentIntentionResult), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  [EndpointName("CreatePaymentIntention")]
  [EndpointSummary("Create a payment intention")]
  [EndpointDescription("Initiates a Paymob payment intention for depositing funds into the user's wallet. Returns the client_secret for the frontend SDK.")]
  public async Task<IActionResult> CreatePaymentIntention(
    [FromBody] CreatePaymentIntentionRequest request,
    CancellationToken ct = default)
  {
    var command = new CreatePaymentIntentionCommand(request.Amount);
    var result = await _sender.Send(command, ct);

    return result.Match(
      intention => Ok(intention),
      errors => Problem(errors));
  }


  /// <summary>
  /// Paymob webhook callback endpoint.
  /// Called by Paymob when a transaction is processed (success/failure).
  /// This endpoint does NOT require authentication — it is secured by HMAC verification.
  /// </summary>
  [AllowAnonymous]
  [HttpPost("paymob/callback")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
  [EndpointName("PaymobCallback")]
  [EndpointSummary("Paymob webhook callback")]
  [EndpointDescription("Receives and processes Paymob transaction callbacks. Secured by HMAC signature verification.")]
  public async Task<IActionResult> PaymobCallback(CancellationToken ct = default)
  {
    // Read raw body for HMAC verification
    using var reader = new StreamReader(Request.Body);
    var rawPayload = await reader.ReadToEndAsync(ct);

    // Extract HMAC from query string (Paymob sends it as ?hmac=...)
    var hmacSignature = Request.Query["hmac"].ToString();

    if (string.IsNullOrEmpty(hmacSignature))
    {
      return BadRequest(new ProblemDetails
      {
        Title = "Missing HMAC signature",
        Detail = "The HMAC signature query parameter is required.",
        Status = StatusCodes.Status400BadRequest
      });
    }

    var command = new ProcessPaymobCallbackCommand(rawPayload, hmacSignature);
    var result = await _sender.Send(command, ct);

    return result.Match(
      _ => Ok(new { message = "Callback processed successfully" }),
      errors => Problem(errors));
  }


  /// <summary>
  /// Paymob redirect endpoint after payment completion.
  /// The frontend should redirect users here and then check the payment status.
  /// </summary>
  [AllowAnonymous]
  [HttpGet("paymob/redirect")]
  [EndpointName("PaymobRedirect")]
  [EndpointSummary("Paymob payment redirect")]
  [EndpointDescription("Handles the redirect from Paymob after payment completion. Returns payment status info.")]
  public IActionResult PaymobRedirect(
    [FromQuery] string? success,
    [FromQuery(Name = "id")] string? transactionId,
    [FromQuery(Name = "order")] string? orderId,
    [FromQuery] string? amount_cents,
    [FromQuery] string? hmac)
  {
    return Ok(new
    {
      message = "Payment redirect received",
      success,
      transactionId,
      orderId,
      amountCents = amount_cents
    });
  }
}


/// <summary>
/// Request model for creating a payment intention.
/// </summary>
public record CreatePaymentIntentionRequest(decimal Amount);
