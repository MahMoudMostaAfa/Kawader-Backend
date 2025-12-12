using Kawadar.Api.Requests;
using Kawadar.Application.Features.Portfolios.Commands.CreateView;
using Kawadar.Application.Features.Portfolios.Queries.GetProjectViews;
using Kawadar.Domain.Portfolios.ProjectView;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/View")]
    public class ViewController : ApiController
    {
        private ISender _sender;

        public ViewController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        [ProducesResponseType(typeof(PortfolioProjectView), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("CreateView")]
        [EndpointSummary("Creates a new view for a project")]
        [EndpointDescription("Creates a new view for a project with the UserId and ProjectId.")]
        public async Task<IActionResult> CreateView(Guid ProjectId, CancellationToken ct)
        {
            var command = new CreateViewCommand(ProjectId);
            var result = await _sender.Send(command, ct);

            return result.Match(
                view => Created(view.UserProfileId.ToString(), view.PortfolioProjectId.ToString())
                , errors => Problem(errors));
        }


        [HttpGet]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetProjectViewsById")]
        [EndpointSummary("Gets project views by its Id")]
        [EndpointDescription("Gets project views by its unique identifier.")]
        public async Task<IActionResult> GetProjectViews(Guid ProjectId, CancellationToken ct)
        {
            var query = new GetProjectViewsQuery(ProjectId);
            var result = await _sender.Send(query, ct);

            return result.Match(
                views => Ok(views)
                , errors => Problem(errors));
        }
    }
}
