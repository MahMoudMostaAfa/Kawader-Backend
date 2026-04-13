using Kawadar.Api.Requests.Skill;
using Kawadar.Application.Features.Skills.Commands.AddSkillsToFreelacner;
using Kawadar.Application.Features.Skills.Commands.AddSkillsToProject;
using Kawadar.Application.Features.Skills.Commands.CreateSkill;
using Kawadar.Application.Features.Skills.Commands.DeleteSkill;
using Kawadar.Application.Features.Skills.Commands.RemoveSkillFromFreelancer;
using Kawadar.Application.Features.Skills.Queries.GetAllSkills;
using Kawadar.Application.Features.Skills.Queries.GetSkillById;
using Kawadar.Domain.Portfolios.ProjectSkill;
using Kawadar.Domain.Skills;
using Kawadar.Domain.Skills.FreelancerSkill;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/Admin/Skill")]
    public class SkillController : ApiController
    {
        private ISender _sender;

        public SkillController(ISender Sender)
        {
            _sender = Sender;
        }


        [HttpGet("{Id:guid}")]
        [ProducesResponseType(typeof(Skill), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetSkillById")]
        [EndpointSummary("Gets a Skill by its Id")]
        [EndpointDescription("Gets a Skill by its unique identifier.")]
        public async Task<IActionResult> GetSkillById(Guid Id, CancellationToken ct)
        {
            var query = new GetSkillByIdQuery(Id);
            var result = await _sender.Send(query, ct);

            return result.Match(
                skill => Ok(skill)
                , errors => Problem(errors));

        }

        [HttpGet("~/api/v{version:apiVersion}/Skill")]
        [ProducesResponseType(typeof(List<Skill>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetAllSkills")]
        [EndpointSummary("Gets All Skills")]
        [EndpointDescription("Gets All Skills")]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var query = new GetAllSkillsQuery();
            var result = await _sender.Send(query, ct);

            return result.Match(
                skills => Ok(skills)
                , errors => Problem(errors));

        }

        [HttpPost("~/api/v{version:apiVersion}/User/Skill")]
        [ProducesResponseType(typeof(List<FreelancerSkill>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("AddsSkillsToFreelancer")]
        [EndpointSummary("Adds Skills for freelancer")]
        [EndpointDescription("Adds Skills for freelancer either predefined or custom")]
        public async Task<IActionResult> AddSkillsToFreelacner([FromBody] AddSkillsToFreelancerRequest request, CancellationToken ct)
        {
            var command = new AddSkillsToFreelacnerCommand(request.skills);
            var result = await _sender.Send(command, ct);

            return result.Match(
                freelancerSkills => Ok(freelancerSkills)
                , errors => Problem(errors));
        }

        [HttpDelete("~/api/v{version:apiVersion}/User/Skill")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("RemoveSkill")]
        [EndpointSummary("Removes a skill from freelancer")]
        [EndpointDescription("Removes a Skill from a freelancer using the skill name")]
        public async Task<IActionResult> RemoveSkillFromFreelancer([FromBody] RemoveSkillFromFreelancerCommand command, CancellationToken ct)
        {
            var result = await _sender.Send(command, ct);

            return result.Match(
                 _ => NoContent()
                , errors => Problem(errors));
        }


        [HttpPost("~/api/v{version:apiVersion}/User/Project/Skill")]
        [ProducesResponseType(typeof(List<PortfolioProjectSkill>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("AddsSkillsToProject")]
        [EndpointSummary("Adds Skills for Project")]
        [EndpointDescription("Adds Skills for project using skills ids")]
        public async Task<IActionResult> AddSkillsToProject([FromBody] AddSkillsToProjectRequest request, CancellationToken ct)
        {
            var command = new AddSkillsToProjectCommand(request.projectId, request.skills!);
            var result = await _sender.Send(command, ct);

            return result.Match(
                freelancerSkills => Ok(freelancerSkills)
                , errors => Problem(errors));
        }

        [HttpPost]
        [ProducesResponseType(typeof(Skill), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("CreateSkill")]
        [EndpointSummary("Creates a skill")]
        [EndpointDescription("Creates a skill with data from the request.")]
        public async Task<IActionResult> Createkill(CreateSkillRequest request, CancellationToken ct)
        {
            var command = new CreateSkillCommand(request.Name, request.isActive);
            var result = await _sender.Send(command, ct);

            return result.Match(
                badge => CreatedAtAction(nameof(GetSkillById), new { Id = badge.Id }, badge)
                , errors => Problem(errors));
        }

        [HttpDelete("{Id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("DeleteSkill")]
        [EndpointSummary("Deletes a Skill")]
        [EndpointDescription("Deletes a skill with Its unique Identifier.")]
        public async Task<IActionResult> DeleteSkill(Guid Id, CancellationToken ct)
        {
            var command = new DeleteSkillCommand(Id);
            var result = await _sender.Send(command, ct);

            return result.Match(

                _ => NoContent()
                , errors => Problem(errors));
        }
    }
}
