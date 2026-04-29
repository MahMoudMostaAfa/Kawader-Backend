
using Kawadar.Application.Features.WalletAndPayments.Queries.GetMyWallet;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1;


[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/wallet")]
public class WalletController : ApiController
{
  private readonly ISender _sender;
  public WalletController(ISender sender)
  {
    _sender = sender;
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
  public async Task<IActionResult> GetWalletSummary(CancellationToken ct = default)
  {
    // Implementation for wallet summary goes here
    return Ok();
  }
}