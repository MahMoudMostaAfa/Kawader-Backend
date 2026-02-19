using Kawadar.Application.Features.ProfileManagment.Commands.UpdateProfile;
using Kawadar.Application.Features.ProfileManagment.Queries.GetUserProfile;
using Kawadar.Application.Features.ProfileManagment.Queries.GetUserProfileByUserName;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Kawadar.Api.Controllers.V1;


[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/profile")]
public class ProfileController : ApiController
{

  private readonly ISender _sender;
  public ProfileController(ISender sender)
  {
    _sender = sender;
  }


  [HttpGet("me")]
  [EndpointName("GetProfile")]
  [EndpointSummary("Gets the profile of the current user")]
  [EndpointDescription("Gets the profile of the current user, including their username, email, and other relevant information.")]
  [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetProfile()
  {

    var profileResult = await _sender.Send(new GetUserProfileQuery());

    return profileResult.Match(
        profile => Ok(profile),
        errors => Problem(errors));
  }

  [HttpGet("{username}")]
  [EndpointName("GetProfileByUsername")]
  [EndpointSummary("Gets the profile of a user by username")]
  [EndpointDescription("Gets the profile of a user by their username, including their email and other relevant information.")]
  [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetProfileByUsername(string username)
  {

    var query = new GetUserProfileByUserNameQuery(username);
    var result = await _sender.Send(query);

    return result.Match(
        profile => Ok(profile),
        errors => Problem(errors));

  }

  [HttpPut("me")]
  [EndpointName("UpdateProfile")]
  [EndpointSummary("Updates the profile of the current user")]
  [EndpointDescription("Updates the profile of the current user with new information.")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> UpdateProfile(UpdateProfileCommand command)
  {
    var result = await _sender.Send(command);

    return result.Match(
        _ => NoContent(),
        errors => Problem(errors));
  }
}