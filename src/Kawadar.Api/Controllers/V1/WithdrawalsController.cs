using Kawadar.Api.Requests.Wallet;
using Kawadar.Application.Features.WalletAndPayments.Commands.CancelWithdrawalRequest;
using Kawadar.Application.Features.WalletAndPayments.Commands.CreateWithdrawalRequest;
using Kawadar.Application.Features.WalletAndPayments.Queries.GetMyWithdrawalRequests;
using Kawadar.Application.Features.WalletAndPayments.Queries.GetWithdrawalRequestById;
using Kawadar.Domain.WalletAndPayments.Payouts.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/wallet")]
public class WithdrawalsController : ApiController
{
  private readonly ISender _sender;

  public WithdrawalsController(ISender sender)
  {
    _sender = sender;
  }

  [HttpPost("withdraw")]
  [EndpointName(nameof(RequestWithdrawal))]
  [EndpointSummary("Request withdrawal")]
  [EndpointDescription("Creates a withdrawal request in Pending status for the authenticated user.")]
  public async Task<IActionResult> RequestWithdrawal([FromBody] CreateWithdrawalRequest request, CancellationToken ct)
  {
    var command = new CreateWithdrawalRequestCommand(request.Amount, request.PayoutAccountId);
    var result = await _sender.Send(command, ct);

    return result.Match(
      withdrawalId => CreatedAtAction(nameof(GetWithdrawalById), new { withdrawalId }, null),
      errors => Problem(errors)
    );
  }

  [HttpGet("withdrawals")]
  [EndpointName(nameof(GetMyWithdrawals))]
  [EndpointSummary("List my withdrawal requests")]
  [EndpointDescription("Returns the authenticated user's withdrawal requests. Optionally filter by status.")]
  public async Task<IActionResult> GetMyWithdrawals([FromQuery] WithdrawalStatus? status, CancellationToken ct)
  {
    var result = await _sender.Send(new GetMyWithdrawalRequestsQuery(status), ct);

    return result.Match(
      withdrawals => Ok(withdrawals),
      errors => Problem(errors)
    );
  }

  [HttpGet("withdrawals/{withdrawalId:guid}")]
  [EndpointName(nameof(GetWithdrawalById))]
  [EndpointSummary("Get withdrawal request details")]
  [EndpointDescription("Returns details of a withdrawal request belonging to the authenticated user.")]
  public async Task<IActionResult> GetWithdrawalById(Guid withdrawalId, CancellationToken ct)
  {
    var result = await _sender.Send(new GetWithdrawalRequestByIdQuery(withdrawalId), ct);

    return result.Match(
      withdrawal => Ok(withdrawal),
      errors => Problem(errors)
    );
  }

  [HttpDelete("withdrawals/{withdrawalId:guid}")]
  [EndpointName(nameof(CancelWithdrawal))]
  [EndpointSummary("Cancel a pending withdrawal")]
  [EndpointDescription("Cancels a pending withdrawal request for the authenticated user.")]
  public async Task<IActionResult> CancelWithdrawal(Guid withdrawalId, CancellationToken ct)
  {
    var result = await _sender.Send(new CancelWithdrawalRequestCommand(withdrawalId), ct);

    return result.Match(
      _ => NoContent(),
      errors => Problem(errors)
    );
  }
}


/// <summary>
/// Controller for handling withdrawal requests, including creating new requests, listing user's requests, getting details of a specific request, and cancelling pending requests.
/// </summary>