using Kawadar.Api.Requests.PortfolioProject.PortfolioItem;
using Kawadar.Application.Features.Portfolios.Commands.CreateItem;
using Kawadar.Application.Features.Portfolios.Commands.DeleteItem;
using Kawadar.Application.Features.Portfolios.Commands.UpdateItem;
using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Application.Features.Portfolios.Queries.GetProjectItemById;
using Kawadar.Application.Features.Portfolios.Queries.GetProjectItemsById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Portfolio/ProjectItem")]
    public class PortfolioItemController : ApiController
    {
        private ISender _sender;
        public PortfolioItemController(ISender sender)
        {
            _sender = sender;
        }


        [HttpPost]
        [ProducesResponseType(typeof(ProjectDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("CreateProjectItem")]
        [EndpointSummary("Creates a project Item")]
        [EndpointDescription("Creates a project item.")]
        public async Task<IActionResult> CreateItem(Guid ProjectId, [FromBody]CreatePortfolioItemRequest request, CancellationToken ct)
        {
            var command = new CreateItemCommand(request.ItemType, request.Content, request.DisplayOrder, ProjectId);
            var result = await _sender.Send(command, ct);

            return result.Match(
                item => CreatedAtAction(nameof(GetProjectItemById), new { Id = item.Id }, item)
                , errors => Problem(errors));
        }


        [HttpGet("{Id:guid}")]
        [ProducesResponseType(typeof(ProjectDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetProjectItemById")]
        [EndpointSummary("Gets a project Item its Id")]
        [EndpointDescription("Gets a project item by its unique identifier.")]
        public async Task<IActionResult> GetProjectItemById(Guid Id, CancellationToken ct)
        {
            var query = new GetProjectItemByIdQuery(Id);
            var result = await _sender.Send(query, ct);

            return result.Match(
                item => Ok(item),
                errors => Problem(errors));
        }


        [HttpGet]
        [ProducesResponseType(typeof(List<ItemDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetItemsById")]
        [EndpointSummary("Gets Project items by Project Id")]
        [EndpointDescription("Gets project items by its unique identifier.")]
        public async Task<IActionResult> GetProjectItemsById(Guid ProjectId, CancellationToken ct)
        {
            var query = new GetProjectWithItemsByIdQuery(ProjectId);
            var result = await _sender.Send(query, ct);

            return result.Match(
                Items => Ok(Items),
                errors => Problem(errors));
        }



        [HttpDelete("{Id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("DeleteItem")]
        [EndpointSummary("Deletes a portfolio project item")]
        [EndpointDescription("Deletes a portfolio project item with Its unique Identifier.")]
        public async Task<IActionResult> DeleteItem(Guid Id, CancellationToken ct)
        {
            var command = new DeleteItemCommand(Id);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }


        [HttpPut("{Id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("UpdateItem")]
        [EndpointSummary("Updates a portfolio project item")]
        [EndpointDescription("Updates a portfolio project item with Its unique Identifier.")]
        public async Task<IActionResult> UpdateItem(Guid Id, [FromBody] UpdateItemRequest request, CancellationToken ct)
        {
            var command = new UpdateItemCommand(Id, request.ItemType, request.Content, request.DisplayOrder);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }


    }
}
