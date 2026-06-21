using Kawadar.Api.Requests.Job;
using Kawadar.Application.Features.Job.Commands.CreateJob;
using Kawadar.Application.Features.Jobs.Commands.AddJobAttachment;
using Kawadar.Application.Features.Jobs.Commands.AddJobQuestion;
using Kawadar.Application.Features.Jobs.Commands.CreateJob.DTOs;
using Kawadar.Application.Features.Jobs.Commands.DeleteJob;
using Kawadar.Application.Features.Jobs.Commands.DeleteJobAttachment;
using Kawadar.Application.Features.Jobs.Commands.DeleteJobQuestion;
using Kawadar.Application.Features.Jobs.Commands.GenerateJobDescription;
using Kawadar.Application.Features.Jobs.Commands.UpdateJob;
using Kawadar.Application.Features.Jobs.Commands.UpdateJobQuestion;
using Kawadar.Application.Features.Jobs.Commands.UpdateJobSkills;
using Kawadar.Application.Features.Jobs.Queries.GetJobBySlug;
using Kawadar.Application.Features.Jobs.Queries.GetJobs;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Jobs.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Kawadar.Application.Features.Reviews.Commands.CreateReview;
using Kawadar.Application.Features.Jobs.Queries.GetJobById;
using Kawadar.Application.Features.Jobs.Queries.GetRecommendationJobs;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.Authorization;

namespace Kawadar.Api.Controllers.V1;


[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/jobs")]
public class JobsController : ApiController
{

  private readonly ISender _sender;
  private readonly ILogger<JobsController> _logger;

  public JobsController(ISender sender, ILogger<JobsController> logger)
  {
    _sender = sender;
    _logger = logger;

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

    var attachmentLinks = request.AttachmentLinks?
        .Where(link => !string.IsNullOrWhiteSpace(link))
        .ToList();

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
        attachmentLinks,
        request.IsPrivate,
        request.PrivateToUserId
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

  [OutputCache(PolicyName = "JobsCachePolicy")]
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

    _logger.LogInformation("Received GetJobs request with search={Search}, specializationId={SpecializationId}, jobType={JobType}, experienceLevel={ExperienceLevel}, budgetRange={BudgetRange}, hourlyRateRange={HourlyRateRange}, skillIds={SkillIds}, page={Page}, pageSize={PageSize}, sortBy={SortBy}",
        search, specilizationId, jobType, experienceLevel, budgetRange, hourlyRateRange, skillIds, page, pageSize, sortBy);

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

  [HttpPost("{slug}/questions")]
  [EndpointSummary("Adds a question to a job")]
  [EndpointDescription("Adds a new question to the job. The display order is automatically set to the next available position. Maximum 5 questions per job. Only the job poster can perform this action.")]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> AddJobQuestion([FromRoute] string slug, [FromBody] AddJobQuestionRequest request, CancellationToken ct)
  {
    var command = new AddJobQuestionCommand(slug, request.Question, request.IsRequired);
    var result = await _sender.Send(command, ct);

    return result.Match(
        created => Created(),
        errors => Problem(errors)
    );
  }

  [HttpPut("{slug}/questions/{questionId:guid}")]
  [EndpointSummary("Updates a job question")]
  [EndpointDescription("Updates the question text and whether it is required. Only the job poster can perform this action.")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> UpdateJobQuestion([FromRoute] string slug, [FromRoute] Guid questionId, [FromBody] UpdateJobQuestionRequest request, CancellationToken ct)
  {
    var command = new UpdateJobQuestionCommand(slug, questionId, request.Question, request.IsRequired);
    var result = await _sender.Send(command, ct);

    return result.Match(
        updated => NoContent(),
        errors => Problem(errors)
    );
  }

  [HttpDelete("{slug}/questions/{questionId:guid}")]
  [EndpointSummary("Deletes a job question")]
  [EndpointDescription("Removes a question from the job and reorders remaining questions. Only the job poster can perform this action.")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> DeleteJobQuestion([FromRoute] string slug, [FromRoute] Guid questionId, CancellationToken ct)
  {
    var command = new DeleteJobQuestionCommand(slug, questionId);
    var result = await _sender.Send(command, ct);

    return result.Match(
        deleted => NoContent(),
        errors => Problem(errors)
    );
  }

  [HttpDelete("{slug}")]
  [EndpointSummary("Deletes a job")]
  [EndpointDescription("Permanently deletes a job and all its attachments from storage. Only the job poster can perform this action.")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> DeleteJob([FromRoute] string slug, CancellationToken ct)
  {
    var command = new DeleteJobCommand(slug);
    var result = await _sender.Send(command, ct);

    return result.Match(
        deleted => NoContent(),
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

  [HttpPost("{slug}/review")]
  [EndpointSummary("Creates a review for a job")]
  [EndpointDescription("Creates a review for a job either client freelancer or freelancer client")]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> ReviewJobBySlug([FromRoute] string slug, [FromBody] ReviewJobRequest request, CancellationToken ct)
  {
    var command = new CreateReviewCommand(slug, request.RevieweeUserName, request.rating, request.Comment);
    var result = await _sender.Send(command, ct);

    return result.Match(
        _ => Created(),
        errors => Problem(errors)
    );
  }

  [HttpPost("generate-description")]
  [EndpointSummary("Generates a job description using AI")]
  [EndpointDescription("Generates a clear and detailed job description based on the provided context. The description is generated in the same language as the context.")]
  [ProducesResponseType(typeof(GeneratedJobDescriptionDto), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> GenerateJobDescription([FromBody] GenerateJobDescriptionRequest request, CancellationToken ct)
  {
    var command = new GenerateJobDescriptionCommand(request.Context);
    var result = await _sender.Send(command, ct);

    return result.Match(
        description => Ok(description),
        errors => Problem(errors)
    );
  }



  [HttpGet("{id:guid}")]
  [EndpointSummary("Gets a job by its ID")]
  [EndpointDescription("Gets full job details including skills, questions, and attachments by its unique ID.")]
  [ProducesResponseType(typeof(JobDetailsDto), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]

  public async Task<IActionResult> GetJobById([FromRoute] Guid id, CancellationToken ct)
  {
    var query = new GetJobByIdQuery(id);

    var result = await _sender.Send(query, ct);

    return result.Match(
          job => Ok(job),
          errors => Problem(errors)
      );
  }

  [HttpGet("recommendations")]
  [EndpointSummary("Gets recommended jobs for a user based on a specific job")]
  [EndpointDescription("Returns a list of jobs recommended for the authenticated user based on their interaction with a specific job identified by its slug. The recommendations are personalized using AI algorithms that analyze the user's behavior and preferences.")]
  [ProducesResponseType(typeof(List<JobSummaryDto>), StatusCodes.Status200OK)]

  public async Task<IActionResult> GetRecommendedJobsForJob(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken ct = default)
  {
    var query = new GetRecommandationJobsQuery(page, pageSize);
    var result = await _sender.Send(query, ct);

    return result.Match(
        jobs => Ok(jobs),
        errors => Problem(errors)
    );
  }



}