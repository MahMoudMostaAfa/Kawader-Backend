using Kawadar.Api.Requests.Admin;
using Kawadar.Api.Requests.Wallet;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.WalletAndPayments.Commands.ApproveWithdrawalRequest;
using Kawadar.Application.Features.WalletAndPayments.Commands.CancelWithdrawalRequest;
using Kawadar.Application.Features.WalletAndPayments.Commands.CreateWithdrawalRequest;
using Kawadar.Application.Features.WalletAndPayments.Commands.RejectWithdrawalRequest;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Application.Features.WalletAndPayments.Queries.GetAdminWithdrawals;
using Kawadar.Application.Features.WalletAndPayments.Queries.GetMyWithdrawalRequests;
using Kawadar.Application.Features.WalletAndPayments.Queries.GetWithdrawalRequestById;
using Kawadar.Domain.Common.Constants;
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

  [HttpGet("/api/v{version:apiVersion}/admin/withdrawals")]
  // [Authorize(Policy = Permissions.ViewWithdrawals)]
  [ProducesResponseType(typeof(PaginatedList<WithdrawalRequestDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  [EndpointName("GetAdminWithdrawals")]
  [EndpointSummary("List admin withdrawals")]
  [EndpointDescription("Lists withdrawal requests for admin review with optional filters.")]
  public async Task<IActionResult> GetAdminWithdrawals(
    [FromQuery] WithdrawalStatus? status,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string sortBy = "newest",
    CancellationToken ct = default)
  {
    var query = new GetAdminWithdrawalsQuery(status, page, pageSize, sortBy);
    var result = await _sender.Send(query, ct);

    return result.Match(
      withdrawals => Ok(withdrawals),
      errors => Problem(errors));
  }

  [HttpPost("/api/v{version:apiVersion}/admin/withdrawals/{withdrawalId:guid}/approve")]
  // [Authorize(Policy = Permissions.ApproveWithdrawals)]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [EndpointName("ApproveWithdrawal")]
  [EndpointSummary("Approve withdrawal")]
  [EndpointDescription("Approves and processes a withdrawal request, creating a wallet transaction and deducting the balance.")]
  public async Task<IActionResult> ApproveWithdrawal([FromRoute] Guid withdrawalId, CancellationToken ct)
  {
    var result = await _sender.Send(new ApproveWithdrawalRequestCommand(withdrawalId), ct);

    return result.Match(
      _ => NoContent(),
      errors => Problem(errors));
  }

  [HttpPost("/api/v{version:apiVersion}/admin/withdrawals/{withdrawalId:guid}/reject")]
  // [Authorize(Policy = Permissions.RejectWithdrawals)]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [EndpointName("RejectWithdrawal")]
  [EndpointSummary("Reject withdrawal")]
  [EndpointDescription("Rejects a withdrawal request with a reason.")]
  public async Task<IActionResult> RejectWithdrawal(
    [FromRoute] Guid withdrawalId,
    [FromBody] RejectWithdrawalRequest request,
    CancellationToken ct)
  {
    var result = await _sender.Send(new RejectWithdrawalRequestCommand(withdrawalId, request.Reason), ct);

    return result.Match(
      _ => NoContent(),
      errors => Problem(errors));
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