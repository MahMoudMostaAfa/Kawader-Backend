using Kawadar.Api.Requests.Violations;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Contracts.Disbutes.Dtos;
using Kawadar.Application.Features.Violations.Commands.SolveViolation;
using Kawadar.Application.Features.Violations.Dtos;
using Kawadar.Application.Features.Violations.Queries.GetAllViolations;
using Kawadar.Application.Features.Violations.Queries.GetViolationById;
using Kawadar.Domain.Common.Constants;
using Kawadar.Domain.Violations.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class ViolationsController : ApiController
    {
        private ISender _sender;

        public ViolationsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("violations/{Id:guid}")]
        [Authorize(Policy = Permissions.ViewViolations)]
        [EndpointSummary("Gets a violation")]
        [EndpointDescription("Get a violation in full detail")]
        [ProducesResponseType(typeof(FullViolationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetViolation([FromRoute] Guid Id, CancellationToken ct = default)
        {
            var query = new GetViolationByIdQuery(Id);
            var result = await _sender.Send(query, ct);

            return result.Match(
                violation => Ok(violation),
                errors => Problem(errors)
            );
        }

        [HttpPut("violations/{Id:guid}")]
        [Authorize(Policy = Permissions.SolveViolations)]
        [EndpointSummary("Solves a violation")]
        [EndpointDescription("Solves a violation by refusing or taking an action against the user")]
        [ProducesResponseType(typeof(fullDisbuteDto), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SolveViolation([FromRoute] Guid Id, [FromBody] SolveViolationRequest request, CancellationToken ct = default)
        {
            var command = new SolveViolationCommand(Id, request.status, request.action!, request.noteByAdmin);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors)
            );
        }

        [HttpGet("violations")]
        [Authorize(Policy = Permissions.ViewViolations)]
        [EndpointSummary("Gets all violations")]
        [EndpointDescription("Get all violations in brief details")]
        [ProducesResponseType(typeof(PaginatedList<BriefViolationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllDisbutes(
            [FromQuery] ViolationStatus? status
            , [FromQuery] ViolationType? type
            , [FromQuery] int page = 1
            , [FromQuery] int pageSize = 10
            , [FromQuery] string sortBy = "newest"
            , CancellationToken ct = default)
        {

            var query = new GetAllViolationsQuery(status, type, page, pageSize, sortBy);
            var result = await _sender.Send(query, ct);

            return result.Match(
                Violations => Ok(Violations),
                errors => Problem(errors)
            );
        }
    }
}
