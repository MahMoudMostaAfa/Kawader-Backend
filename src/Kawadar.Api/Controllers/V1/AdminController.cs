using Kawadar.Api.Requests.Admin;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Admins.Commands.AddClaim;
using Kawadar.Application.Features.Admins.Commands.BanUser;
using Kawadar.Application.Features.Admins.Commands.CreateAdmin;
using Kawadar.Application.Features.Admins.Commands.DeleteUser;
using Kawadar.Application.Features.Admins.Dtos;
using Kawadar.Application.Features.Admins.Queries.GetAdmins;
using Kawadar.Application.Features.Admins.Queries.GetUsers;
using Kawadar.Domain.Common.Constants;
using Kawadar.Domain.UserProfiles.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Admin")]
    public class AdminController : ApiController
    {
        private ISender _sender;

        public AdminController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("Users")]
        [Authorize(Policy = Permissions.ViewUsers)]
        [ProducesResponseType(typeof(PaginatedList<UserProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetUsers")]
        [EndpointSummary("Gets the users and their data")]
        [EndpointDescription("Gets the users and their data so that the admin can manage the users")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] bool? IsDeleted,
            [FromQuery] bool? IsBanned,
            [FromQuery] ExperienceYear? ExperienceYear,
            [FromQuery] Guid? specilizationId,
            [FromQuery] int page =1,
            [FromQuery] int pageSize =10,
            [FromQuery] string sortBy ="newest",
            CancellationToken ct = default)
        {
            var query = new GetUserProfilesQuery(IsDeleted,
                IsBanned,
                ExperienceYear,
                specilizationId,
                page,
                pageSize,
                sortBy);
            var result = await _sender.Send(query, ct);

            return result.Match(
                UserProfiles => Ok(UserProfiles)
                , errors => Problem(errors));

        }

        [HttpGet("Admins")]
        [Authorize(Policy = Permissions.ViewAdmins)]
        [ProducesResponseType(typeof(PaginatedList<AdminDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetAdmins")]
        [EndpointSummary("Gets the Admins and their data")]
        [EndpointDescription("Gets the Admins and their data so that the Master admin can manage the Admins")]
        public async Task<IActionResult> GetAdmins(
            [FromQuery] bool? IsOnline,
            [FromQuery] bool? IsDeleted,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "newest",
            CancellationToken ct = default)
        {
            var query = new GetAdminsQuery(IsOnline,
                IsDeleted,
                page,
                pageSize,
                sortBy);
            var result = await _sender.Send(query, ct);

            return result.Match(
                Admins => Ok(Admins)
                , errors => Problem(errors));

        }

        [HttpPut("Ban")]
        [Authorize(Policy = Permissions.BanUsers)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("BansUser")]
        [EndpointSummary("Bans user by their userName")]
        [EndpointDescription("Bans the user by their username for a period of time")]
        public async Task<IActionResult> BanUser([FromBody] BanUserRequest request, CancellationToken ct)
        {
            var command = new BanUserCommand(request.UserName, request.BannedUntil);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }

        [HttpPut("Delete")]
        [Authorize(Policy = Permissions.DeleteUsers)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("DeletesUser")]
        [EndpointSummary("Deletes user by their userName")]
        [EndpointDescription("Soft delete the user by their username for a period of time")]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest request, CancellationToken ct)
        {
            var command = new DeleteUserCommand(request.UserName);
            var result = await _sender.Send(command, ct);

            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }

        [HttpPost("Add")]
        [Authorize(Policy = Permissions.AddAdmin)]
        [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("Add admin")]
        [EndpointSummary("Adds a new Admin.")]
        [EndpointDescription("Adds a new Admin account with the provided registration details.")]
        public async Task<IActionResult> Add([FromBody] CreateAdminCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return result.Match(
              _ => Created(),
              errors => Problem(errors)
            );
        }

        [HttpPost("AddPermission")]
        [Authorize(Policy = Permissions.AddClaim)]
        [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("AddPermission")]
        [EndpointSummary("Adds a permission to an Admin.")]
        [EndpointDescription("Adds a permission to an Admin using its userName.")]
        public async Task<IActionResult> AddPermission([FromBody] AddClaimCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return result.Match(
              _ => Created(),
              errors =>Problem(errors)
            );
        }
    }
}
