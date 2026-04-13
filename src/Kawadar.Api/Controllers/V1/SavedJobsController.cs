using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Application.Features.Jobs.SavedJobs.Commands.AddSavedJob;
using Kawadar.Application.Features.Jobs.SavedJobs.Commands.RemoveSavedJob;
using Kawadar.Application.Features.Jobs.SavedJobs.Queries.GetSavedJobsByUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/saved-jobs")]
[Authorize]
public class SavedJobsController : ApiController
{

  private readonly ISender _sender;

  public SavedJobsController(ISender sender)
  {
    _sender = sender;
  }
  /// <summary>
  /// Saves a job for the current authenticated user.
  /// </summary>
  /// <remarks>
  /// Creates a saved-job relation between the current user and the provided job ID.
  /// </remarks>
  [HttpPost("{jobId:guid}", Name = "AddSavedJob")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> AddSavedJob([FromRoute] Guid jobId, CancellationToken cancellationToken)
  {
    var command = new AddSavedJobCommand(jobId);
    var result = await _sender.Send(command, cancellationToken);
    return result.Match(
      _ => NoContent(),
      errors => Problem(errors)
    );
  }

  /// <summary>
  /// Removes a saved job for the current authenticated user.
  /// </summary>
  /// <remarks>
  /// Deletes the saved-job relation between the current user and the provided job ID.
  /// </remarks>
  [HttpDelete("{jobId:guid}", Name = "RemoveSavedJob")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> RemoveSavedJob([FromRoute] Guid jobId, CancellationToken cancellationToken)
  {

    var command = new RemoveSavedJobCommand(jobId);

    var result = await _sender.Send(command, cancellationToken);
    return result.Match(
      _ => NoContent(),
      errors => Problem(errors)
    );
  }

  /// <summary>
  /// Gets paginated saved jobs for the current authenticated user.
  /// </summary>
  /// <remarks>
  /// Returns a paginated list of jobs saved by the current user.
  /// </remarks>
  [HttpGet(Name = "GetSavedJobs")]
  [ProducesResponseType(typeof(PaginatedList<JobSummaryDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> GetSavedJobs([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
  {
    var query = new GetSavedJobsByUserQuery(pageNumber, pageSize);
    var result = await _sender.Send(query, CancellationToken.None);
    return result.Match(
      paginatedList => Ok(paginatedList),
      errors => Problem(errors)
    );
  }
}