using Kawadar.Api.Requests.Disbutes;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Contracts.Disbutes.Commands.RaiseDisbute;
using Kawadar.Application.Features.Contracts.Disbutes.Commands.SolveDisbute;
using Kawadar.Application.Features.Contracts.Disbutes.Dtos;
using Kawadar.Application.Features.Contracts.Disbutes.Queries.GetDisbuteById;
using Kawadar.Application.Features.Contracts.Disbutes.Queries.GetDisbutes;
using Kawadar.Domain.Common.Constants;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Contracts.Disbutes.Enum;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class DisbuteController : ApiController
    {
        private ISender _sender;

        public DisbuteController(ISender Sender)
        {
            _sender = Sender;
        }

        [HttpPost("contracts/{contractId:guid}/disbute")]
        [Authorize]
        [ProducesResponseType(typeof(Result<Success>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("CreateDisbute")]
        [EndpointSummary("Creates a disbute")]
        [EndpointDescription("Creates a disbute with data from the request.")]
        public async Task<IActionResult> RaiseDisbute([FromRoute] Guid contractId, RaiseDisbuteRequest request, CancellationToken ct)
        {
            var command = new RaiseDisbuteCommand(contractId, request.reason!);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => Created(),
                errors => Problem(errors));
        }

        [HttpGet("disbutes/{Id:guid}")]
        [Authorize(Policy = Permissions.ViewDisbutes)]
        [EndpointSummary("Gets a disbute")]
        [EndpointDescription("Get a disbute in full detail")]
        [ProducesResponseType(typeof(fullDisbuteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDisbute([FromRoute] Guid Id, CancellationToken ct = default)
        {
            var query = new GetDisbuteByIdQuery(Id);
            var result = await _sender.Send(query, ct);

            return result.Match(
                userReport => Ok(userReport),
                errors => Problem(errors)
            );
        }

        [HttpPut("disbutes/{Id:guid}")]
        [Authorize(Policy = Permissions.SolveDisbutes)]
        [EndpointSummary("Solves a disbute")]
        [EndpointDescription("Solves a disbute by setting the resolution and the status")]
        [ProducesResponseType(typeof(fullDisbuteDto), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SolveDisbute([FromRoute] Guid Id, [FromBody] solveDisbuteRequest request, CancellationToken ct = default)
        {
            var command = new SolveDisbuteCommand(Id, request.status, request.resolution);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors)
            );
        }

        [HttpGet("disbutes")]
        [Authorize(Policy = Permissions.ViewDisbutes)]
        [EndpointSummary("Gets all disbutes")]
        [EndpointDescription("Get all disbute in brief details")]
        [ProducesResponseType(typeof(PaginatedList<BriefDisbuteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllDisbutes(
            [FromQuery] DisbuteStatus? status
            ,[FromQuery] int page = 1
            ,[FromQuery] int pageSize = 10
            ,[FromQuery] string sortBy = "newest"
            ,CancellationToken ct = default)
        {

            var query = new GetDisbutesQuery(status, page, pageSize, sortBy);
            var result = await _sender.Send(query, ct);

            return result.Match(
                Disbutes => Ok(Disbutes),
                errors => Problem(errors)
            );
        }
    }
}
