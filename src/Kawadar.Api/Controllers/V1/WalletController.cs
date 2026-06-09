using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Application.Features.WalletAndPayments.Queries.GetAdminWalletById;
using Kawadar.Application.Features.WalletAndPayments.Queries.GetAdminWallets;
using Kawadar.Application.Features.WalletAndPayments.Queries.GetAllTransactions;
using Kawadar.Application.Features.WalletAndPayments.Queries.GetAllWalletTransactions;
using Kawadar.Application.Features.WalletAndPayments.Queries.GetMyWallet;
using Kawadar.Application.Features.WalletAndPayments.Queries.GetWalletSummary;
using Kawadar.Domain.Common.Constants;
using Kawadar.Domain.WalletAndPayments.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1;


[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/wallet")]
[Tags("Wallet")]
public class WalletController : ApiController
{
  private readonly ISender _sender;
  public WalletController(ISender sender)
  {
    _sender = sender;
  }

  [HttpGet("/api/v{version:apiVersion}/admin/wallets")]
  [Authorize(Policy = Permissions.ViewWallets)]
  [ProducesResponseType(typeof(PaginatedList<AdminWalletDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  [EndpointName("GetAdminWallets")]
  [EndpointSummary("Lists all wallets")]
  [EndpointDescription("Lists all wallets with optional filters for admin review.")]
  public async Task<IActionResult> GetAdminWallets(
    [FromQuery] Guid? userId,
    [FromQuery] bool? isActive,
    [FromQuery] decimal? minBalance,
    [FromQuery] decimal? maxBalance,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string sortBy = "newest",
    CancellationToken ct = default)
  {
    var query = new GetAdminWalletsQuery(userId, isActive, minBalance, maxBalance, page, pageSize, sortBy);
    var result = await _sender.Send(query, ct);

    return result.Match(
      wallets => Ok(wallets),
      errors => Problem(errors));
  }

  [HttpGet("/api/v{version:apiVersion}/admin/transactions")]
  [Authorize(Policy = Permissions.ViewTransactions)]
  [ProducesResponseType(typeof(PaginatedList<TransactionDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  [EndpointName("GetAllTransaction")]
  [EndpointSummary("Lists all Transactions")]
  [EndpointDescription("Lists all Transactions with optional filters for admin review.")]
  public async Task<IActionResult> GetAllTransactions(
  [FromQuery] TransactionType? type,
  [FromQuery] WalletTransactionStatus? status,
  [FromQuery] WalletTransactionReferenceType? reference,
  [FromQuery] int page = 1,
  [FromQuery] int pageSize = 10,
  [FromQuery] string sortBy = "newest",
  CancellationToken ct = default)
  {
    var query = new GetAllTransactionsQuery(type, status, reference, page, pageSize, sortBy);
    var result = await _sender.Send(query, ct);

    return result.Match(
      Transactions => Ok(Transactions),
      errors => Problem(errors));
  }

  [HttpGet("/api/v{version:apiVersion}/admin/transactions/{walletId:guid}")]
  [Authorize(Policy = Permissions.ViewTransactions)]
  [ProducesResponseType(typeof(PaginatedList<TransactionDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  [EndpointName("GetAllWalletTransaction")]
  [EndpointSummary("Lists all transactions from a single wallet")]
  [EndpointDescription("Lists all transactions from a single wallet with optional filters for admin review.")]
  public async Task<IActionResult> GetAllWalletTransactions(
  [FromRoute] Guid walletId,
  [FromQuery] TransactionType? type,
  [FromQuery] WalletTransactionStatus? status,
  [FromQuery] WalletTransactionReferenceType? reference,
  [FromQuery] int page = 1,
  [FromQuery] int pageSize = 10,
  [FromQuery] string sortBy = "newest",
  CancellationToken ct = default)
  {
    var query = new GetAllWalletTransactionsQuery(walletId, type, status, reference, page, pageSize, sortBy);
    var result = await _sender.Send(query, ct);

    return result.Match(
      Transactions => Ok(Transactions),
      errors => Problem(errors));
  }

  [HttpGet("/api/v{version:apiVersion}/admin/wallets/{walletId:guid}")]
  [Authorize(Policy = Permissions.ViewWallets)]
  [ProducesResponseType(typeof(AdminWalletDto), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  [EndpointName("GetAdminWalletById")]
  [EndpointSummary("Gets wallet details")]
  [EndpointDescription("Gets wallet details for any user by wallet ID.")]
  public async Task<IActionResult> GetAdminWalletById([FromRoute] Guid walletId, CancellationToken ct = default)
  {
    var query = new GetAdminWalletByIdQuery(walletId);
    var result = await _sender.Send(query, ct);

    return result.Match(
      wallet => Ok(wallet),
      errors => Problem(errors));
  }
  [HttpGet]
  [EndpointName(nameof(GetMyWallet))]
  public async Task<IActionResult> GetMyWallet(CancellationToken ct = default)
  {

    var result = await _sender.Send(new GetMyWalletQuery(), ct);
    return result.Match(
        wallet => Ok(wallet),
        error => Problem(error)
    );
  }

  [HttpGet("summary")]
  [ProducesResponseType(typeof(PaginatedList<TransactionDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  [EndpointName("GetWalletSummary")]
  [EndpointSummary("Gets wallet transaction summary")]
  [EndpointDescription("Gets the current user's wallet transactions as a paginated list with optional filters.")]
  public async Task<IActionResult> GetWalletSummary(
    [FromQuery] TransactionType? type,
    [FromQuery] WalletTransactionStatus? status,
    [FromQuery] WalletTransactionReferenceType? referenceType,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string sortBy = "newest",
    CancellationToken ct = default)
  {
    var query = new GetWalletSummaryQuery(type, status, referenceType, page, pageSize, sortBy);
    var result = await _sender.Send(query, ct);

    return result.Match(
      transactions => Ok(transactions),
      errors => Problem(errors));
  }


}