

using Kawadar.Api.Requests.Contracts;
using Kawadar.Application.Features.Contracts.Commands.CreateContract;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/contracts")]
public class ContractsController : ApiController
{


  private readonly ISender _sender;

  public ContractsController(ISender sender)
  {
    _sender = sender;

  }
  [HttpPost]
  public async Task<IActionResult> CreateContract([FromBody] CreateContractRequest request, CancellationToken ct)
  {
    var command = new CreateContractCommand(request.JobId, request.ProposaslId, request.ContractType, request.StartDate);
    var result = await _sender.Send(command, ct);

    return result.Match(
      contractId => CreatedAtAction(nameof(GetContractById), new { id = contractId }, null),
      errors => Problem(errors)
    );

  }

  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetContractById(Guid id, CancellationToken ct)
  {
    // 7f91e5dc-bdbf-4e1e-8f03-34204ea17781
    // Implementation for getting a contract by ID goes here
    return Ok();

  }

}