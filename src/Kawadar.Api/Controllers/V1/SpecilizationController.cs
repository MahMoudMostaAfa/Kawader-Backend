using Kawadar.Api.Requests.Specilization;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Features.Specilizations.Commands.CreateSpecilization;
using Kawadar.Application.Features.Specilizations.Commands.DeleteSpecilization;
using Kawadar.Application.Features.Specilizations.Commands.UpdateSpecilization;
using Kawadar.Application.Features.Specilizations.DTO;
using Kawadar.Application.Features.Specilizations.Queries.GetAllSpecilizations;
using Kawadar.Application.Features.Specilizations.Queries.GetSpecilizationById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Specilization")]
    public class SpecilizationController : ApiController
    {
        private ISender _sender;
        public SpecilizationController(ISender sender)
        {
            _sender = sender;
        }


        [HttpPost]
        [ProducesResponseType(typeof(SpecilizationDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("CreateSpecilization")]
        [EndpointSummary("Creates a new Specilization")]
        [EndpointDescription("Creates a new specilization with data from the command.")]
        public async Task<IActionResult> CreateSpecilization([FromBody]CreateSpecilizationRequest request, CancellationToken ct)
        {
            var command = new CreateSpecilizationCommand(request.Name, request.IsActive);
            var result = await _sender.Send(command, ct);

            return result.Match(
                 specilization => CreatedAtAction(nameof(GetSpecilizationById), new { Id = specilization.Id }, specilization)
                , errors => Problem(errors));
        }


        [HttpGet("{Id:guid}")]
        [ProducesResponseType(typeof(SpecilizationDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetSpecilizationById")]
        [EndpointSummary("Gets a Specilization by its Id")]
        [EndpointDescription("Gets a specilization by its unique identifier.")]
        public async Task<IActionResult> GetSpecilizationById(Guid Id, CancellationToken ct)
        {
            var query = new GetSpecilizationByIdQuery(Id);
            var result = await _sender.Send(query, ct);

            return result.Match(
                specilizaiton => Ok(specilizaiton),
                errors => Problem(errors));
        }


        [HttpGet]
        [ProducesResponseType(typeof(List<SpecilizationDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetAll")]
        [EndpointSummary("Gets all specilizations")]
        [EndpointDescription("Gets all specilizaions")]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var query = new GetAllSpecilizationsQuery();
            var result = await _sender.Send(query, ct);

            return result.Match(
                specilizations => Ok(specilizations)
                , errors => Problem(errors));
        }


        [HttpDelete("{Id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("DeleteSpecilization")]
        [EndpointSummary("Deletes a specilization")]
        [EndpointDescription("Deletes a specilization with Its unique Identifier.")]
        public async Task<IActionResult> DeleteSpecilization(Guid Id, CancellationToken ct)
        {
            var command = new DeleteSpecilizationCommand(Id);
            var result = await _sender.Send(command, ct);

            return result.Match(

                _ => NoContent()
                , errors => Problem(errors));
        }


        [HttpPut("{Id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("UpdateSpecilization")]
        [EndpointSummary("Updates a specilization")]
        [EndpointDescription("Updates a specilization with Its unique Identifier.")]
        public async Task<IActionResult> UpdateSpecilization(Guid Id, [FromBody]UpdateSpecilizationRequest request, CancellationToken ct)
        {
            var command = new UpdateSpecilizationCommand(Id, request.Name, request.IsActive);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent()
                , errors => Problem(errors));
        }
    }
}
