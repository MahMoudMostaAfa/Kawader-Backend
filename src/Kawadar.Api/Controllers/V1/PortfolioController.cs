using Kawadar.Api.Requests.PortfolioProject;
using Kawadar.Application.Features.Portfolios.Commands.CreateProject;
using Kawadar.Application.Features.Portfolios.Commands.DeleteProject;
using Kawadar.Application.Features.Portfolios.Commands.OrderPortfolioProjects;
using Kawadar.Application.Features.Portfolios.Commands.UpdateProject;
using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Application.Features.Portfolios.Queries.GetAllProjectsByFreelancerId;
using Kawadar.Application.Features.Portfolios.Queries.GetProjectById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/User/Portfolio")]
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
            var command = new CreateProjectCommand(request.title, request.description, request.category, request.ProjectImage, request.ProjectUrl);
            var result = await _sender.Send(command, ct);

            return result.Match(
                project => CreatedAtAction(nameof(GetProjectById), new { Id = project.Id }, project),
                errors => Problem(errors));
        }


        [HttpGet("{Id:guid}")]
        [ProducesResponseType(typeof(ProjectDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetProjectById")]
        [EndpointSummary("Gets a project by its Id")]
        [EndpointDescription("Gets a project by its unique identifier.")]
        public async Task<IActionResult> GetProjectById(Guid Id, CancellationToken ct)
        {
            var query = new GetProjectByIdQuery(Id);
            var result = await _sender.Send(query, ct);

            return result.Match(
                project => Ok(project),
                errors => Problem(errors));
        }


        [HttpGet]
        [ProducesResponseType(typeof(List<ProjectDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetProjectsById")]
        [EndpointSummary("Gets projects by freelancer Id")]
        [EndpointDescription("Gets freelancer projects by his unique identifier.")]
        public async Task<IActionResult> GetAllPortfolioProjectsById(Guid Id, CancellationToken cancellationToken)
        {
            var query = new GetAllProjectsByFreelancerIdQuery(Id);
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
        public async Task<IActionResult> DeleteProject(Guid Id, CancellationToken ct)
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
        public async Task<IActionResult> UpdateProject(Guid Id, [FromForm] UpdateProjectRequest request, CancellationToken ct)
        {
            var command = new UpdateProjectCommand(Id, request.ProjectUrl, request.Image, request.isPublic);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }

        [HttpPut("Reorder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("ReorderProjects")]
        [EndpointSummary("Reorders the Portfolio Projects")]
        [EndpointDescription("Reorders the Portfolio Projects using Ids and new display order")]
        public async Task<IActionResult> ReorderProjects([FromBody] ReorderProjectsRequest request, CancellationToken ct)
        {
            var command = new OrderPortfolioProjectsCommand(request.Order);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }

    }
}
