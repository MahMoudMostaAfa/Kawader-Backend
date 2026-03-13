using Kawadar.Api.Requests.Proposals;
using Kawadar.Application.Features.Proposals.Commands.CreateProposal;
using Kawadar.Application.Features.Proposals.Commands.DeleteProposal;
using Kawadar.Application.Features.Proposals.Commands.UpdateProposal;
using Kawadar.Application.Features.Proposals.Queries.GetProposalById;
using MassTransit.Futures.Contracts;
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

  [HttpPut("proposals/{proposalId:guid}")]
  public async Task<IActionResult> UpdateProposal([FromRoute] Guid proposalId, [FromBody] UpdateProposalRequest updateProposalRequest)
  {
    var command = new UpdateProposalCommand(
        proposalId,
        updateProposalRequest.CoverLetter,
        updateProposalRequest.QuestionAnswerUpdateDtos,
        updateProposalRequest.MilestoneUpdateDtos,
        updateProposalRequest.Amount,
        updateProposalRequest.EstimatedDays,
        updateProposalRequest.HourlyRate,
        updateProposalRequest.EstimatedHours
    );

    var result = await _sender.Send(command);

    return result.Match(_ => NoContent(),
    err => Problem(err));
  }

  [HttpGet("proposals/{proposalId:guid}")]
  public async Task<IActionResult> GetProposalById([FromRoute] Guid proposalId)
  {
    var result = await _sender.Send(new GetProposalByIdQuery(proposalId));

    return result.Match(proposal => Ok(proposal), err => Problem(err));
  }


  [HttpDelete("proposals/{proposalId:guid}")]

  public async Task<IActionResult> DeleteProposal([FromRoute] Guid proposalId)
  {

    var result = await _sender.Send(new DeleteProposalCommand(proposalId));

    return result.Match(_ => NoContent(), err => Problem(err));
  }

}