using Kawadar.Api.Requests.Proposals;
using Kawadar.Application.Features.Proposals.Commands.CreateProposal;
using Kawadar.Application.Features.Proposals.Commands.DeleteProposal;
using Kawadar.Application.Features.Proposals.Commands.UpdateProposal;
using Kawadar.Application.Features.Proposals.Queries.GetProposalById;
using Kawadar.Application.Features.Proposals.Queries.GetProposals;
using Kawadar.Application.Features.Proposals.Queries.GetUserProposals;
using Kawadar.Domain.Proposals.Enums;
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
  [HttpPost("jobs/{jobId:guid}/proposals")]
  public async Task<IActionResult> AddProposal([FromRoute] Guid jobId, [FromBody] CreateProposalRequest request)
  {
    var command = new CreateProposalCommand(jobId, request.CoverLetter, request.JobProposalType, request.Amount, request.EstimatedDays, request.HourlyRate, request.EstimatedHours, request.QuestionAnswerDtos, request.MilestoneDtos);

    var result = await _sender.Send(command);

    return result.Match(
        created => Created(),
        errors => Problem(errors));
  }

  [HttpGet("jobs/{jobId:guid}/proposals")]

  public async Task<IActionResult> GetProposals(
  [FromRoute] Guid jobId,
  // options for filtering and sorting , newest, oldest, price low to high, price high to low, estimated time low to high, estimated time high to low
  [FromQuery] JobProposalType? type,
  [FromQuery] JobProposalStatus? status,
  [FromQuery] int page = 1,
  [FromQuery] int pageSize = 10,
  [FromQuery] string datesortBy = "newest",
  [FromQuery] string? priceSortBy = null,
  [FromQuery] string? estimatedTimeSortBy = null
  )
  {

    var query = new GetProposalsQuery(jobId, type, status, page, pageSize, datesortBy, priceSortBy, estimatedTimeSortBy);

    var result = await _sender.Send(query);

    return result.Match(
      items => Ok(items),
      err => Problem(err)
    );
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


  [HttpGet("proposals/my-proposals")]
  public async Task<IActionResult> GetMyProposals(
    [FromQuery] string sortBy = "newest",
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10
  )
  {
    var query = new GetUserProposalsQuery(sortBy, pageSize, page);

    var result = await _sender.Send(query);

    return result.Match(

      data => Ok(data),
      err => Problem(err)
    );
  }

}