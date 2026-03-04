using Kawadar.Api.Requests.Job;
using Kawadar.Application.Features.Job.Commands.CreateJob;
using Kawadar.Application.Features.Jobs.Commands.AddJobAttachment;
using Kawadar.Application.Features.Jobs.Commands.CreateJob.DTOs;
using Kawadar.Application.Features.Jobs.Commands.DeleteJobAttachment;
using Kawadar.Application.Features.Jobs.Commands.UpdateJob;
using Kawadar.Application.Features.Jobs.Commands.UpdateJobQuestions;
using Kawadar.Application.Features.Jobs.Commands.UpdateJobSkills;
using Kawadar.Application.Features.Jobs.Queries.GetJobBySlug;
using Kawadar.Application.Features.Jobs.Queries.GetJobs;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Jobs.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1;


[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/jobs")]
public class JobsController : ApiController
{

  private readonly ISender _sender;

  public JobsController(ISender sender)
  {
    _sender = sender;

  }

  [HttpPost]
  [Consumes("multipart/form-data")]
  [EndpointSummary("Creates a new job")]
  [EndpointDescription("Creates a new job posting with title, description, skills, questions, and attachments.")]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> CreateJob([FromForm] CreateJobRequest request, CancellationToken ct)
  {
    Console.WriteLine($"Received CreateJobRequest: Title={request.Title}, SkillIds={request.SkillIds}, AttachmentFilesCount={(request.AttachmentFiles?.Count ?? 0)}, AttachmentLinksCount={(request.AttachmentLinks?.Count ?? 0)}");
    var skillIds = request.SkillIds
        ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(s => Guid.TryParse(s, out var g) ? g : Guid.Empty)
        .Where(g => g != Guid.Empty)
        .ToList() ?? [];

    var questionDtos = request.Questions?
        .Select((q, i) => new CreateQuestionDto(q, request.QuestionsRequired?.ElementAtOrDefault(i) ?? false))
        .ToList() ?? [];

    var command = new CreateJobCommand(
        request.Title,
        request.Description,
        request.SpecilizationId,
        (JobType)request.JobType,
        (BudgetRange)request.BudgetRange,
        (HourlyRateRange)request.HourlyRateRange,
        request.DurationInDays,
        (JobExperienceLevel)request.ExperienceLevel,
        questionDtos,
        skillIds,
        request.AttachmentFiles,
        request.AttachmentLinks
    );

    var result = await _sender.Send(command, ct);

    return result.Match(
        created => CreatedAtAction(nameof(GetJobs), new { }, null),
        errors => Problem(errors)
    );

  }

  [HttpPut("{slug}")]
  [EndpointSummary("Updates a job by its slug")]
  [EndpointDescription("Updates job details such as title, description, type, budget, and experience level.")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> UpdateJob([FromRoute] string slug, [FromBody] UpdateJobRequest request, CancellationToken ct)
  {
    var command = new UpdateJobCommand(
        slug,
        request.Title,
        request.Description,
        request.SpecilizationId,
        request.JobType,
        request.BudgetRange,
        request.HourlyRateRange,
        request.DurationInDays,
        request.ExperienceLevel

    );

    var result = await _sender.Send(command, ct);

    return result.Match(
        updated => NoContent(),
        errors => Problem(errors)
    );
  }




  [HttpGet("{slug}")]
  [EndpointSummary("Gets a job by its slug")]
  [EndpointDescription("Gets full job details including skills, questions, and attachments by its unique slug.")]
  [ProducesResponseType(typeof(JobDetailsDto), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> GetJobBySlug(string slug, CancellationToken ct)
  {
    var query = new GetJobBySlugQuery(slug);
    var result = await _sender.Send(query, ct);

    return result.Match(
        job => Ok(job),
        errors => Problem(errors)
    );
  }

  [HttpGet]
  [EndpointSummary("Lists and searches jobs")]
  [EndpointDescription("Returns a paginated list of jobs with optional filtering by search term, specialization, type, experience level, budget range, hourly rate, and skills.")]
  [ProducesResponseType(typeof(PaginatedList<JobSummaryDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> GetJobs(
    [FromQuery] string? search,
    [FromQuery] Guid? specilizationId,
    [FromQuery] JobType? jobType,
    [FromQuery] JobExperienceLevel? experienceLevel,
    [FromQuery] BudgetRange? budgetRange,
    [FromQuery] HourlyRateRange? hourlyRateRange,
    [FromQuery] string? skillIds,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string sortBy = "newest",
    CancellationToken ct = default)
  {
    var parsedSkillIds = skillIds
        ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(s => Guid.TryParse(s, out var g) ? g : Guid.Empty)
        .Where(g => g != Guid.Empty)
        .ToList();

    var query = new GetJobsQuery(
        search,
        specilizationId,
        jobType,
        experienceLevel,
        budgetRange,
        hourlyRateRange,
        parsedSkillIds,
        page,
        pageSize,
        sortBy
    );

    var result = await _sender.Send(query, ct);

    return result.Match(
        jobs => Ok(jobs),
        errors => Problem(errors)
    );
  }

  [HttpPut("{slug}/skills")]
  [EndpointSummary("Replaces job skills")]
  [EndpointDescription("Replaces all skills of a job with the provided list. Only the job poster can perform this action.")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> UpdateJobSkills([FromRoute] string slug, [FromBody] UpdateJobSkillsRequest request, CancellationToken ct)
  {
    var command = new UpdateJobSkillsCommand(slug, request.SkillIds);
    var result = await _sender.Send(command, ct);

    return result.Match(
        updated => NoContent(),
        errors => Problem(errors)
    );
  }

  [HttpPut("{slug}/questions")]
  [EndpointSummary("Replaces job questions")]
  [EndpointDescription("Updates, adds, or removes job questions. Existing questions are matched by Id; new questions have a null Id. Questions not in the payload are removed. Only the job poster can perform this action.")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> UpdateJobQuestions([FromRoute] string slug, [FromBody] UpdateJobQuestionsRequest request, CancellationToken ct)
  {
    var command = new UpdateJobQuestionsCommand(
        slug,
        request.Questions.Select(q => new UpdateQuestionItemDto(q.Id, q.Question, q.IsRequired)).ToList()
    );
    var result = await _sender.Send(command, ct);

    return result.Match(
        updated => NoContent(),
        errors => Problem(errors)
    );
  }

  [HttpPost("{slug}/attachments")]
  [Consumes("multipart/form-data")]
  [EndpointSummary("Adds an attachment to a job")]
  [EndpointDescription("Uploads a file or adds an external URL as an attachment to the job. Only the job poster can perform this action. Maximum 5 attachments per job.")]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> AddJobAttachment([FromRoute] string slug, [FromForm] AddJobAttachmentRequest request, CancellationToken ct)
  {
    var command = new AddJobAttachmentCommand(slug, request.File, request.ExternalUrl);
    var result = await _sender.Send(command, ct);

    return result.Match(
        created => Created(),
        errors => Problem(errors)
    );
  }

  [HttpDelete("{slug}/attachments/{attachmentId:guid}")]
  [EndpointSummary("Deletes a job attachment")]
  [EndpointDescription("Removes an attachment from the job and deletes the file from storage if it was uploaded. Only the job poster can perform this action.")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> DeleteJobAttachment([FromRoute] string slug, [FromRoute] Guid attachmentId, CancellationToken ct)
  {
    var command = new DeleteJobAttachmentCommand(slug, attachmentId);
    var result = await _sender.Send(command, ct);

    return result.Match(
        deleted => NoContent(),
        errors => Problem(errors)
    );
  }
}