using Kawadar.Api.Requests.PayoutAccounts;
using Kawadar.Application.Features.WalletAndPayments.Commands.AddPayoutAccount;
using Kawadar.Application.Features.WalletAndPayments.Commands.DeactivatePayoutAccount;
using Kawadar.Application.Features.WalletAndPayments.Commands.SetDefaultPayoutAccount;
using Kawadar.Application.Features.WalletAndPayments.Commands.UpdatePayoutAccount;
using Kawadar.Application.Features.WalletAndPayments.Queries.GetMyPayoutAccounts;
using Kawadar.Application.Features.WalletAndPayments.Queries.GetPayoutAccountById;
using Kawadar.Domain.WalletAndPayments.Payouts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/wallet/payout-accounts")]
public class PayoutAccountsController : ApiController
{
  private readonly ISender _sender;

  public PayoutAccountsController(ISender sender)
  {
    _sender = sender;
  }

  [HttpPost]
  [EndpointName(nameof(AddPayoutAccount))]
  [EndpointSummary("Add a new payout account")]
  [EndpointDescription("Adds a new payout account (MobileWallet, BankTransfer, InstaPay) for the authenticated user.")]
  public async Task<IActionResult> AddPayoutAccount([FromBody] AddPayoutAccountRequest request, CancellationToken ct)
  {
    // Map the incoming request to the command, handling the polymorphic account details

    PayoutAccountDetails? accountDetails = request.PayoutType switch
    {
      Domain.WalletAndPayments.Payouts.Enums.PayoutType.MobileWallet => request.MobileWalletAccountDetails,
      Domain.WalletAndPayments.Payouts.Enums.PayoutType.BankTransfer => request.BankTransferAccountDetails,
      Domain.WalletAndPayments.Payouts.Enums.PayoutType.InstaPay => request.InstaPayAccountDetails,
      _ => null
    };
    var command = new AddPayoutAccountCommand(
      request.PayoutType,
      request.DisplayName,
      accountDetails,
      request.IsDefault
    );
    var result = await _sender.Send(command, ct);

    return result.Match(
      accountId => CreatedAtAction(nameof(GetPayoutAccountById), new { accountId }, null),
      errors => Problem(errors)
    );
  }

  [HttpGet]
  [EndpointName(nameof(GetMyPayoutAccounts))]
  [EndpointSummary("List my payout accounts")]
  [EndpointDescription("Returns a list of active payout accounts for the authenticated user.")]
  public async Task<IActionResult> GetMyPayoutAccounts(CancellationToken ct)
  {
    var result = await _sender.Send(new GetMyPayoutAccountsQuery(), ct);

    return result.Match(
      accounts => Ok(accounts),
      errors => Problem(errors)
    );
  }

  [HttpGet("{accountId:guid}")]
  [EndpointName(nameof(GetPayoutAccountById))]
  [EndpointSummary("Get payout account details")]
  [EndpointDescription("Returns details of a specific payout account belonging to the authenticated user.")]
  public async Task<IActionResult> GetPayoutAccountById(Guid accountId, CancellationToken ct)
  {
    var query = new GetPayoutAccountByIdQuery(accountId);
    var result = await _sender.Send(query, ct);

    return result.Match(
      account => Ok(account),
      errors => Problem(errors)
    );
  }

  [HttpPut("{accountId:guid}")]
  [EndpointName(nameof(UpdatePayoutAccount))]
  [EndpointSummary("Update payout account")]
  [EndpointDescription("Updates the display name, account details, and default flag of a payout account.")]
  public async Task<IActionResult> UpdatePayoutAccount([FromRoute] Guid accountId, [FromBody] UpdatePayoutAccountRequest request, CancellationToken ct)
  {
    // Map the incoming request to the command, handling the polymorphic account details
    PayoutAccountDetails? accountDetails = request.MobileWalletAccountDetails as PayoutAccountDetails
      ?? request.BankTransferAccountDetails as PayoutAccountDetails
      ?? request.InstaPayAccountDetails as PayoutAccountDetails;
    var command = new UpdatePayoutAccountCommand(
      accountId,
      request.DisplayName,
      accountDetails,
      request.IsDefault
    );
    var result = await _sender.Send(command, ct);

    return result.Match(
      _ => NoContent(),
      errors => Problem(errors)
    );
  }

  [HttpDelete("{accountId:guid}")]
  [EndpointName(nameof(DeactivatePayoutAccount))]
  [EndpointSummary("Deactivate payout account")]
  [EndpointDescription("Soft deletes a payout account by setting IsActive to false.")]
  public async Task<IActionResult> DeactivatePayoutAccount(Guid accountId, CancellationToken ct)
  {
    var command = new DeactivatePayoutAccountCommand(accountId);
    var result = await _sender.Send(command, ct);

    return result.Match(
      _ => NoContent(),
      errors => Problem(errors)
    );
  }

  [HttpPatch("{accountId:guid}/set-default")]
  [EndpointName(nameof(SetDefaultPayoutAccount))]
  [EndpointSummary("Set as default payout account")]
  [EndpointDescription("Sets the specified payout account as the default for the authenticated user.")]
  public async Task<IActionResult> SetDefaultPayoutAccount(Guid accountId, CancellationToken ct)
  {
    var command = new SetDefaultPayoutAccountCommand(accountId);
    var result = await _sender.Send(command, ct);

    return result.Match(
      _ => NoContent(),
      errors => Problem(errors)
    );
  }
}
