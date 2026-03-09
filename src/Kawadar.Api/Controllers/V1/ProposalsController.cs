using Kawadar.Api.Requests.Proposals;
using Kawadar.Application.Features.Proposals.Commands.CreateProposal;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1;

[Authorize]
[Route("api/v{version:apiVersion}")]
public class ProposalsController : ApiController
{

  private readonly ISender _sender;

  public ProposalsController(ISender sender)
  {
    _sender = sender;
  }
  [HttpPost("job/{jobId:guid}/proposals")]
  public async Task<IActionResult> AddProposal([FromRoute] Guid jobId, [FromBody] CreateProposalRequest request)
  {
    var command = new CreateProposalCommand(jobId, request.CoverLetter, request.JobProposalType, request.Amount, request.EstimatedDays, request.HourlyRate, request.EstimatedHours, request.QuestionAnswerDtos, request.MilestoneDtos);

    var result = await _sender.Send(command);

    return result.Match(
        created => Created(),
        errors => Problem(errors));
  }

}