using Kawadar.Api.Requests.PortfolioProject;
using Kawadar.Api.Requests.PortfolioProject.PortfolioItem;
using Kawadar.Application.Features.Portfolios.Commands.CreateItem;
using Kawadar.Application.Features.Portfolios.Commands.CreateProject;
using Kawadar.Application.Features.Portfolios.Commands.DeleteItem;
using Kawadar.Application.Features.Portfolios.Commands.DeleteProject;
using Kawadar.Application.Features.Portfolios.Commands.OrderPortfolioProjects;
using Kawadar.Application.Features.Portfolios.Commands.OrderProjectItems;
using Kawadar.Application.Features.Portfolios.Commands.UpdateItem;
using Kawadar.Application.Features.Portfolios.Commands.UpdateProject;
using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Application.Features.Portfolios.Queries.GetAllProjectsByFreelancerId;
using Kawadar.Application.Features.Portfolios.Queries.GetProjectItemsById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Portfolio")]
    public class PortfolioController : ApiController
    {
        private ISender _sender;
        public PortfolioController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ProjectDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("CreateProject")]
        [EndpointSummary("Creates a new portfolio project")]
        [EndpointDescription("Creates a new portfolio project with the freelancerId.")]
        public async Task<IActionResult> CreatePortfolioProject([FromForm] CreateProjectRequest request, CancellationToken ct = default)
        {
            var command = new CreateProjectCommand(request.title, request.description, request.specilizationName, request.ProjectImage!, request.ProjectUrl!);
            var result = await _sender.Send(command, ct);

            return result.Match(
                project => Created(),
                errors => Problem(errors));
        }


        [HttpGet("{Id:guid}/items")]
        [ProducesResponseType(typeof(List<ItemDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetItemsById")]
        [EndpointSummary("Gets Project items by Project Id")]
        [EndpointDescription("Gets project items by its unique identifier.")]
        public async Task<IActionResult> GetProjectItemsById([FromRoute] Guid Id, CancellationToken ct = default)
        {
            var query = new GetProjectWithItemsByIdQuery(Id);
            var result = await _sender.Send(query, ct);

            return result.Match(
                Items => Ok(Items),
                errors => Problem(errors));
        }

        [HttpGet("{userName}")]
        [ProducesResponseType(typeof(List<ProjectDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetProjectsByUserName")]
        [EndpointSummary("Gets projects by freelancer UserName")]
        [EndpointDescription("Gets freelancer projects by his unique UserName.")]
        public async Task<IActionResult> GetAllPortfolioProjectsByUserName([FromRoute] string userName, CancellationToken cancellationToken = default)
        {
            var query = new GetAllProjectsByFreelancerUserNameQuery(userName);
            var result = await _sender.Send(query, cancellationToken);

            return result.Match(
                projects => Ok(projects),
                errors => Problem(errors));
        }


        [HttpDelete("{Id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("DeleteProject")]
        [EndpointSummary("Deletes a portfolio project")]
        [EndpointDescription("Deletes a portfolio project with Its unique Identifier.")]
        public async Task<IActionResult> DeleteProject([FromRoute] Guid Id, CancellationToken ct = default)
        {
            var command = new DeleteProjectCommand(Id);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }


        [HttpPut("{Id:guid}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("UpdateProject")]
        [EndpointSummary("Updates a portfolio project")]
        [EndpointDescription("Updates a portfolio project with Its unique Identifier.")]
        public async Task<IActionResult> UpdateProject([FromRoute] Guid Id, [FromForm] UpdateProjectRequest request, CancellationToken ct = default)
        {
            var command = new UpdateProjectCommand(Id, request.ProjectUrl, request.Image!, request.isPublic);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("ReorderProjects")]
        [EndpointSummary("Reorders the Portfolio Projects")]
        [EndpointDescription("Reorders the Portfolio Projects using Ids and new display order")]
        public async Task<IActionResult> ReorderProjects([FromBody] ReorderProjectsRequest request, CancellationToken ct = default)
        {
            var command = new OrderPortfolioProjectsCommand(request.Order!);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }

        [HttpPost("{Id:guid}/items")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("CreateProjectItem")]
        [EndpointSummary("Creates a project Item")]
        [EndpointDescription("Creates a project item.")]
        public async Task<IActionResult> CreateItem([FromRoute] Guid Id, [FromForm] CreatePortfolioItemRequest request, CancellationToken ct = default)
        {
            var command = new CreateItemCommand(request.ItemType, request.Content, request.Image, Id);
            var result = await _sender.Send(command, ct);

            return result.Match(
                item => Created()
                , errors => Problem(errors));
        }

        [HttpDelete("{Id:guid}/items/{ItemId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("DeleteItem")]
        [EndpointSummary("Deletes a portfolio project item")]
        [EndpointDescription("Deletes a portfolio project item with Its unique Identifier.")]
        public async Task<IActionResult> DeleteItem([FromRoute] Guid ItemId, CancellationToken ct = default)
        {
            var command = new DeleteItemCommand(ItemId);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }

        [HttpPut("{Id:guid}/items/{ItemId:guid}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("UpdateItem")]
        [EndpointSummary("Updates a portfolio project item")]
        [EndpointDescription("Updates a portfolio project item with Its unique Identifier.")]
        public async Task<IActionResult> UpdateItem([FromRoute] Guid ItemId, [FromForm] UpdateItemRequest request, CancellationToken ct = default)
        {
            var command = new UpdateItemCommand(ItemId, request.itemType, request.Content, request.Image);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }

        [HttpPut("{Id:guid}/items")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("ReorderItems")]
        [EndpointSummary("Reorder project items")]
        [EndpointDescription("Reorder project items using Ids and display order")]
        public async Task<IActionResult> UpdateImageItem([FromRoute] Guid Id, [FromBody] OrderProjectItemsRequest request, CancellationToken ct = default)
        {
            var command = new OrderProjectItemsCommand(Id, request.Order!);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }
    }
}