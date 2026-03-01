using Kawadar.Api.Requests.Job;
using Kawadar.Application.Features.Job.Commands.CreateJob;
using Kawadar.Application.Features.Jobs.Commands.CreateJob.DTOs;
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




  [HttpGet("{slug}")]
  public async Task<IActionResult> GetJobBySlug(string slug, CancellationToken ct)
  {
    await Task.Delay(100, ct);

    return Ok();

  }
  [HttpGet]
  public async Task<IActionResult> GetJobs(CancellationToken ct)
  {
    await Task.Delay(100, ct);

    return Ok();


  }
}