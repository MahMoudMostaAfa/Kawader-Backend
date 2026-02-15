using Kawadar.Api.Requests.Badge;
using Kawadar.Application.Features.Badges.Commands.CreateBadge;
using Kawadar.Application.Features.Badges.Commands.DeleteBadge;
using Kawadar.Application.Features.Badges.Commands.UpdateBadge;
using Kawadar.Application.Features.Badges.DTOs;
using Kawadar.Application.Features.Badges.Queries;
using Kawadar.Domain.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Badge")]
    public class BadgeController : ApiController
    {
        private ISender _sender;

        public BadgeController(ISender Sender)
        {
            _sender = Sender;
        }

        [HttpGet("{Id:guid}")]
        [Authorize(Policy = Permissions.ViewBadges)]
        [ProducesResponseType(typeof(BadgeDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetBadgeById")]
        [EndpointSummary("Gets a badge by its Id")]
        [EndpointDescription("Gets a badge by its unique identifier.")]
        public async Task<IActionResult> GetBadgeById(Guid Id, CancellationToken ct)
        {
            var query = new GetBadgeByIdQuery(Id);
            var result = await _sender.Send(query, ct);

            return result.Match(
                badge => Ok(badge)
                , errors => Problem(errors));
        }


        [HttpPost]
        [Authorize(Policy = Permissions.CreateBadges)]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(BadgeDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("CreateBadge")]
        [EndpointSummary("Creates a badge")]
        [EndpointDescription("Creates a badge with data from the request.")]
        public async Task<IActionResult> CreateBadge([FromForm]CreateBadgeRequest request, CancellationToken ct)
        {
            var command = new CreateBadgeCommand(request.title, request.Icon, request.description);
            var result = await _sender.Send(command, ct);

            return result.Match(
                badge => CreatedAtAction(nameof(GetBadgeById), new { Id = badge.Id }, badge)
                , errors => Problem(errors));
        }



        [HttpDelete("{Id:guid}")]
        [Authorize(Policy = Permissions.DeleteBadges)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("DeleteBadge")]
        [EndpointSummary("Deletes a badge by its Id")]
        [EndpointDescription("Deletes a badge by its unique identifier.")]
        public async Task<IActionResult> DeleteBadge(Guid Id, CancellationToken ct)
        {
            var command = new DeleteBadgeCommand(Id);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent()
                , errors => Problem(errors));
        }


        [HttpPut("{Id:guid}")]
        [Authorize(Policy = Permissions.EditBadges)]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("UpdateBadge")]
        [EndpointSummary("Updates a badge by its Id")]
        [EndpointDescription("Updates a badge by its unique identifier.")]
        public async Task<IActionResult> UpdateBadge(Guid Id, [FromForm]UpdateBadgeRequest request, CancellationToken ct)
        {
            var command = new UpdateBadgeCommand(Id, request.Icon);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent()
                , errors => Problem(errors));
        }

    }
}
