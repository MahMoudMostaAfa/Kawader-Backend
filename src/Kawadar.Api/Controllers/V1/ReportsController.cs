using Kawadar.Api.Requests.Job;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Jobs.Commands.ReportJob;
using Kawadar.Application.Features.Jobs.Commands.UpdateJobReport;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Application.Features.Jobs.Queries.GetJobReport;
using Kawadar.Application.Features.Jobs.Queries.GetJobReports;
using Kawadar.Application.Features.Jobs.Queries.GetReportsByJobSlug;
using Kawadar.Application.Features.ProfileManagment.Commands.ReportUser;
using Kawadar.Application.Features.ProfileManagment.Commands.UpdateUserReport;
using Kawadar.Application.Features.ProfileManagment.DTOs;
using Kawadar.Application.Features.ProfileManagment.Queries.GetUserReport;
using Kawadar.Application.Features.ProfileManagment.Queries.GetUserReportByUserName;
using Kawadar.Application.Features.ProfileManagment.Queries.GetUserReports;
using Kawadar.Domain.Common.Constants;
using Kawadar.Domain.Jobs.JobReports.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [EndpointSummary("Reports a job")]
        [EndpointDescription("Reports a job by identifiying the type of violation")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReportJob([FromRoute] string slug, [FromBody] ReportRequest request, CancellationToken ct)
        {
            var command = new ReportJobCommand(slug, request.reportType, request.content);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => Created(),
                errors => Problem(errors)
            );
        }

        [HttpPost("User/{userName}")]
        [Authorize]
        [EndpointSummary("Reports a User")]
        [EndpointDescription("Reports a User by identifiying the type of violation")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReportUser([FromRoute] string userName, [FromBody] ReportRequest request, CancellationToken ct)
        {
            var command = new ReportUserCommand(userName, request.content, request.reportType);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => Created(),
                errors => Problem(errors)
            );
        }

        [HttpGet("User")]
        [Authorize]
        [EndpointSummary("Gets Users Reports")]
        [EndpointDescription("Gets Users Reports in pages with brief information")]
        [ProducesResponseType(typeof(PaginatedList<BriefUserReportDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserReports(
        [FromQuery] ReportType? reportType,
        [FromQuery] ReportStatus? reportStatus,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "newest", CancellationToken ct = default)
        {
            var query = new GetUserReportsQuery(reportType, reportStatus, page, pageSize, sortBy);
            var result = await _sender.Send(query, ct);

            return result.Match(
                userReports => Ok(userReports),
                errors => Problem(errors)
            );
        }

        [HttpGet("User/{userName}")]
        [Authorize]
        [EndpointSummary("Gets Users Reports By userName")]
        [EndpointDescription("Gets a certain Users Reports by userName in pages with brief information")]
        [ProducesResponseType(typeof(PaginatedList<BriefUserReportDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserReportsByUserName(
        [FromRoute] string userName,
        [FromQuery] ReportType? reportType,
        [FromQuery] ReportStatus? reportStatus,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "newest", CancellationToken ct = default)
        {
            var query = new GetUserReportByUserNameQuery(reportType, reportStatus, userName, page, pageSize, sortBy);
            var result = await _sender.Send(query, ct);

            return result.Match(
                userReports => Ok(userReports),
                errors => Problem(errors)
            );
        }

        [HttpGet("User/{Id:guid}")]
        [Authorize]
        [EndpointSummary("Gets a User Report")]
        [EndpointDescription("Get a User Report in full detail")]
        [ProducesResponseType(typeof(FullJobReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserReport([FromRoute]Guid Id, CancellationToken ct = default)
        {
            var query = new GetUserReportQuery(Id);
            var result = await _sender.Send(query, ct);

            return result.Match(
                userReport => Ok(userReport),
                errors => Problem(errors)
            );
        }

        [HttpPut("User/{Id:guid}")]
        [Authorize]
        [EndpointSummary("Updates a User Report")]
        [EndpointDescription("Updates a User Report Status and Action Taken")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserReport([FromRoute] Guid Id, UpdateReportRequest request, CancellationToken ct = default)
        {
            var command = new UpdateUserReportCommand(Id, request.reportStatus, request.ActionTaken);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors)
            );
        }

        [HttpGet]
        [Authorize(Policy = Permissions.ViewJobReports)]
        [EndpointSummary("Gets Jobs Reports")]
        [EndpointDescription("Gets Jobs Reports in pages with brief information")]
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
        [Authorize(Policy = Permissions.ViewJobReports)]
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
        [Authorize(Policy = Permissions.UpdateJobReports)]
        [EndpointSummary("Updates a Job Report")]
        [EndpointDescription("Updates Job Report Status and Action Taken")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateJobReport([FromRoute] Guid Id, UpdateReportRequest request, CancellationToken ct = default)
        {
            var command = new UpdateJobReportCommand(Id, request.ActionTaken, request.reportStatus);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors)
            );
        }

        [HttpGet("{slug}")]
        [Authorize(Policy = Permissions.ViewJobReports)]
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
