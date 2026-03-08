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
using Kawadar.Application.Features.Jobs.Commands.ReportJob;
using Kawadar.Domain.Jobs.JobReports.Enums;
using Kawadar.Application.Features.Jobs.Queries.GetJobReports;
using Kawadar.Application.Features.Jobs.Queries.GetJobReport;
using Kawadar.Application.Features.Jobs.Commands.UpdateJobReport;
using Kawadar.Application.Features.Jobs.Queries.GetReportsByJobSlug;

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

    [HttpPost("{slug}/Report")]
    [EndpointSummary("Reports a job")]
    [EndpointDescription("Reports a job by identifiying the type of violation")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ReportJob([FromRoute]string slug, [FromBody]ReportJobRequest request, CancellationToken ct)
    {
        var command = new ReportJobCommand(slug, request.reportType, request.content);
        var result = await _sender.Send(command, ct);

        return result.Match(
            _ => Created(),
            errors => Problem(errors)
        );
    }

    [HttpGet("Reports")]
    [EndpointSummary("Gets Job Reports")]
    [EndpointDescription("Gets Job Reports in pages with brief information")]
    [ProducesResponseType(typeof(PaginatedList<BriefJobReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetJobReports(
        [FromQuery] ReportType? reportType,
        [FromQuery] ReportStatus? reportStatus,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "newest", CancellationToken ct = default)
    {
        var query = new GetJobReportsQuery(reportType, reportStatus, page, pageSize, sortBy);
        var result = await _sender.Send(query, ct);

        return result.Match(
            jobReports => Ok(jobReports),
            errors => Problem(errors)
        );
    }

    [HttpGet("Reports/{Id:guid}")]
    [EndpointSummary("Gets a Job Report")]
    [EndpointDescription("Gets a Job Report by its unique identifier with full details")]
    [ProducesResponseType(typeof(FullJobReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetJobReport([FromRoute] Guid Id , CancellationToken ct = default)
    {
        var query = new GetJobReportQuery(Id);
        var result = await _sender.Send(query, ct);

        return result.Match(
            jobReport => Ok(jobReport),
            errors => Problem(errors)
        );
    }

    [HttpPut("Reports/{Id:guid}")]
    [EndpointSummary("Updates a Job Report")]
    [EndpointDescription("Updates Job Report Status and Action Taken")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateJobReport([FromRoute] Guid Id, UpdateJobReportRequest request, CancellationToken ct = default)
    {
        var command = new UpdateJobReportCommand(Id, request.ActionTaken, request.reportStatus);
        var result = await _sender.Send(command, ct);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }

    [HttpGet("{slug}/Reports")]
    [EndpointSummary("Gets job reports")]
    [EndpointDescription("Gets job reports using the job slug")]
    [ProducesResponseType(typeof(List<BriefJobReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetReportsByJobSlug([FromRoute] string slug, CancellationToken ct)
    {
        var query = new GetReportsByJobSlugQuery(slug);
        var result = await _sender.Send(query, ct);

        return result.Match(
            reports => Ok(reports),
            errors => Problem(errors)
        );
    }
}