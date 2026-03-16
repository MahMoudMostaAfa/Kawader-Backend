using Kawadar.Api.Requests.Job;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Jobs.Commands.ReportJob;
using Kawadar.Application.Features.Jobs.Commands.UpdateJobReport;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Application.Features.Jobs.Queries.GetJobReport;
using Kawadar.Application.Features.Jobs.Queries.GetJobReports;
using Kawadar.Application.Features.Jobs.Queries.GetReportsByJobSlug;
using Kawadar.Domain.Jobs.JobReports.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1
{
    
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Reports")]
    public class ReportsController : ApiController
    {
        private ISender _sender;
        public ReportsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("{slug}")]
        [EndpointSummary("Reports a job")]
        [EndpointDescription("Reports a job by identifiying the type of violation")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReportJob([FromRoute] string slug, [FromBody] ReportJobRequest request, CancellationToken ct)
        {
            var command = new ReportJobCommand(slug, request.reportType, request.content);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => Created(),
                errors => Problem(errors)
            );
        }

        [HttpGet]
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

        [HttpGet("{Id:guid}")]
        [EndpointSummary("Gets a Job Report")]
        [EndpointDescription("Gets a Job Report by its unique identifier with full details")]
        [ProducesResponseType(typeof(FullJobReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetJobReport([FromRoute] Guid Id, CancellationToken ct = default)
        {
            var query = new GetJobReportQuery(Id);
            var result = await _sender.Send(query, ct);

            return result.Match(
                jobReport => Ok(jobReport),
                errors => Problem(errors)
            );
        }

        [HttpPut("{Id:guid}")]
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

        [HttpGet("{slug}")]
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
}
