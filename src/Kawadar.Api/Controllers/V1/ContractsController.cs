

using Kawadar.Api.Requests.Contracts;
using Kawadar.Application.Features.Contracts.Commands.AcceptContractCompletion;
using Kawadar.Application.Features.Contracts.Commands.CancelContract;
using Kawadar.Application.Features.Contracts.Commands.CreateContract;
using Kawadar.Application.Features.Contracts.Commands.EditContractDeadline;
using Kawadar.Application.Features.Contracts.Commands.RejectContractCompletion;
using Kawadar.Application.Features.Contracts.Commands.RequestContractCompletion;
using Kawadar.Application.Features.Contracts.Milestones.Commands.DeleteContractMilestone;
using Kawadar.Application.Features.Contracts.Milestones.Commands.ApproveContractMilestone;
using Kawadar.Application.Features.Contracts.Milestones.Commands.RejectContractMilestone;
using Kawadar.Application.Features.Contracts.Milestones.Commands.StartContractMilestone;
using Kawadar.Application.Features.Contracts.Milestones.Commands.SubmitContractMilestone;
using Kawadar.Application.Features.Contracts.Milestones.Commands.UpdateContractMilestone;
using Kawadar.Application.Features.Contracts.Milestones.Queries.GetContractMilestoneById;
using Kawadar.Application.Features.Contracts.Milestones.Queries.GetContractMilestones;
using Kawadar.Application.Features.Contracts.Queries.GetContractDetails;
using Kawadar.Application.Features.Contracts.Queries.GetMyContracts;
using MassTransit.Futures.Contracts;
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
    var query = new GetContractDetailsQuery(id);
    var result = await _sender.Send(query, ct);


    return result.Match(
      contractDetails => Ok(contractDetails),
      errors => Problem(errors)
    );

  }

  [HttpGet("{contractId:guid}/milestones")]
  public async Task<IActionResult> GetContractMilestones([FromRoute] Guid contractId, CancellationToken ct)
  {
    var query = new GetContractMilestonesQuery(contractId);
    var result = await _sender.Send(query, ct);

    return result.Match(
      milestones => Ok(milestones),
      errors => Problem(errors)
    );
  }

  [HttpGet("{contractId:guid}/milestones/{milestoneId:guid}")]
  public async Task<IActionResult> GetContractMilestoneById([FromRoute] Guid contractId, [FromRoute] Guid milestoneId, CancellationToken ct)
  {
    var query = new GetContractMilestoneByIdQuery(contractId, milestoneId);
    var result = await _sender.Send(query, ct);

    return result.Match(
      milestone => Ok(milestone),
      errors => Problem(errors)
    );
  }

  [HttpPut("{contractId:guid}/milestones/{milestoneId:guid}")]
  public async Task<IActionResult> UpdateContractMilestone([FromRoute] Guid contractId, [FromRoute] Guid milestoneId, [FromBody] UpdateContractMilestoneRequest request, CancellationToken ct)
  {
    var command = new UpdateContractMilestoneCommand(contractId, milestoneId, request.DueDate);
    var result = await _sender.Send(command, ct);

    return result.Match(
      _ => NoContent(),
      errors => Problem(errors)
    );
  }

  [HttpDelete("{contractId:guid}/milestones/{milestoneId:guid}")]
  public async Task<IActionResult> DeleteContractMilestone([FromRoute] Guid contractId, [FromRoute] Guid milestoneId, CancellationToken ct)
  {
    var command = new DeleteContractMilestoneCommand(contractId, milestoneId);
    var result = await _sender.Send(command, ct);

    return result.Match(
      _ => NoContent(),
      errors => Problem(errors)
    );
  }

  [HttpPost("{contractId:guid}/milestones/{milestoneId:guid}/start")]
  public async Task<IActionResult> StartContractMilestone([FromRoute] Guid contractId, [FromRoute] Guid milestoneId, CancellationToken ct)
  {
    var command = new StartContractMilestoneCommand(contractId, milestoneId);
    var result = await _sender.Send(command, ct);

    return result.Match(
      _ => NoContent(),
      errors => Problem(errors)
    );
  }

  [HttpPost("{contractId:guid}/milestones/{milestoneId:guid}/submit")]
  public async Task<IActionResult> SubmitContractMilestone([FromRoute] Guid contractId, [FromRoute] Guid milestoneId, CancellationToken ct)
  {
    var command = new SubmitContractMilestoneCommand(contractId, milestoneId);
    var result = await _sender.Send(command, ct);

    return result.Match(
      _ => NoContent(),
      errors => Problem(errors)
    );
  }

  [HttpPost("{contractId:guid}/milestones/{milestoneId:guid}/approve")]
  public async Task<IActionResult> ApproveContractMilestone([FromRoute] Guid contractId, [FromRoute] Guid milestoneId, CancellationToken ct)
  {
    var command = new ApproveContractMilestoneCommand(contractId, milestoneId);
    var result = await _sender.Send(command, ct);

    return result.Match(
      _ => NoContent(),
      errors => Problem(errors)
    );
  }

  [HttpPost("{contractId:guid}/milestones/{milestoneId:guid}/reject")]
  public async Task<IActionResult> RejectContractMilestone([FromRoute] Guid contractId, [FromRoute] Guid milestoneId, [FromBody] RejectContractMilestoneRequest request, CancellationToken ct)
  {
    var command = new RejectContractMilestoneCommand(contractId, milestoneId, request.Reason);
    var result = await _sender.Send(command, ct);

    return result.Match(
      _ => NoContent(),
      errors => Problem(errors)
    );
  }

  [HttpGet]
  [EndpointName(nameof(GetMyContracts))]
  [EndpointSummary("Get a list of my (client -freelancer) contracts")]
  [EndpointDescription("Returns a list of contracts associated with the authenticated user, either as a client or freelancer.")]
  public async Task<IActionResult> GetMyContracts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
  {
    var query = new GetMyContractsQuery(page, pageSize);
    var result = await _sender.Send(query, ct);


    return result.Match(
      contracts => Ok(contracts),
      errors => Problem(errors)
    );

  }


  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> CancelContract(Guid id, CancellationToken ct)
  {
    var command = new CancelContractCommand(id);
    var result = await _sender.Send(command, ct);

    return result.Match(
      _ => NoContent(),
      errors => Problem(errors)
    );
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> EditContractDeadline([FromRoute] Guid id, [FromBody] EditContractDeadlineRequest request, CancellationToken ct)
  {
    var command = new EditContractDeadlineCommand(id, request.NewDeadline);
    var result = await _sender.Send(command, ct);
    return result.Match(
      _ => NoContent(),
      errors => Problem(errors)
    );
  }

  [HttpPost("{id:guid}/request-completion")]
  public async Task<IActionResult> RequestContractCompletion(Guid id, CancellationToken ct)
  {
    var command = new RequesContractCompletionCommand(id);
    var result = await _sender.Send(command, ct);
    return result.Match(
      _ => NoContent(),
      errors => Problem(errors)
    );
  }
  [HttpPost("{id:guid}/reject-completion")]
  public async Task<IActionResult> RejectContractCompletion([FromRoute] Guid id, [FromBody] RejectCompletionRequest request, CancellationToken ct)
  {

    var command = new RejectContractCompletionCommand(id, request.Reason);
    var result = await _sender.Send(command, ct);
    return result.Match(
      _ => NoContent(),
      errors => Problem(errors)
    );


  }


  [HttpPost("{id:guid}/accept-completion")]
  public async Task<IActionResult> AcceptContractCompletion(Guid id, CancellationToken ct)
  {

    var command = new AcceptContractCompletionCommand(id);
    var result = await _sender.Send(command, ct);

    return result.Match(
      _ => NoContent(),
      errors => Problem(errors)
    );
  }
}