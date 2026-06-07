
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Badges.Queries.GetFreelancerBadgesQuery;
using Kawadar.Application.Features.ProfileManagment.Commands.UpdateProfile;
using Kawadar.Application.Features.ProfileManagment.Commands.UpdateProfileImage;
using Kawadar.Application.Features.ProfileManagment.Commands.UploadIdentity;
using Kawadar.Application.Features.ProfileManagment.Queries.GetFreelancers;
using Kawadar.Application.Features.ProfileManagment.Queries.GetFreelancersByAi;
using Kawadar.Application.Features.ProfileManagment.Queries.GetUserProfile;
using Kawadar.Application.Features.ProfileManagment.Queries.GetUserProfileByUserName;
using Kawadar.Application.Features.Reviews.Dtos;
using Kawadar.Application.Features.Reviews.Queries.GetReviewsByUserProfileId;
using Kawadar.Application.Features.Reviews.Queries.GetReviewStatistics;
using Kawadar.Domain.UserProfiles.Enums;
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
  private readonly IAIService _aiService;

  public ProfileController(ISender sender, IAIService aiService)
  {
    _sender = sender;
    _aiService = aiService;


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

  [HttpGet("Freelancers")]
  [EndpointName("GetFreelancers")]
  [EndpointSummary("Gets the freelancers according to the sent paramaters")]
  [EndpointDescription("Gets the freelancers according to the sent paramaters in a brief description.")]
  [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> GetFreelancers(
      [FromQuery] float? AverageRating,
      [FromQuery] ExperienceYear? ExperienceYear,
      [FromQuery] Guid? specilizationId,
      [FromQuery] int page = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string sortBy = "newest",
      CancellationToken ct = default
      )
  {
    var query = new GetFreelancersQuery(ExperienceYear, specilizationId, AverageRating, page, pageSize, sortBy);
    var profileResult = await _sender.Send(query, ct);

    return profileResult.Match(
        freelancers => Ok(freelancers),
        errors => Problem(errors));
  }


  [HttpGet("ai-search")]
  public async Task<IActionResult> GetFreelancersByAi([FromQuery] string query)
  {
    var querySender = new GetFreelancersByAiQuery(query);
    var result = await _sender.Send(querySender);

    return result.Match(
        freelancers => Ok(freelancers),
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

  [HttpGet("{username}/reviews")]
  [EndpointName("UserReviews")]
  [EndpointSummary("Gets the reviews of a user by username")]
  [EndpointDescription("Gets the reviews of a user by their username.")]
  [ProducesResponseType(typeof(PaginatedList<ReviewDto>), StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetUserReviews([FromRoute] string username,
      [FromQuery] float? rating,
      [FromQuery] int page = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string sortBy = "highest")
  {

    var query = new GetReviewsByUserNameQuery(rating, page, pageSize, sortBy, username);
    var result = await _sender.Send(query);

    return result.Match(
        reviews => Ok(reviews),
        errors => Problem(errors));

  }


  [HttpGet("{username}/statistics")]
  [EndpointName("UserStatistics")]
  [EndpointSummary("Gets the review statistics of a user by username")]
  [EndpointDescription("Gets the review statistics(The count and average rating) of a user by their username.")]
  [ProducesResponseType(typeof(ReviewStatisticsDto), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetStatistics([FromRoute] string username)
  {

    var query = new GetReviewStatisticsQuery(username);
    var result = await _sender.Send(query);

    return result.Match(
        statistics => Ok(statistics),
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



  [Consumes("multipart/form-data")]
  [HttpPost("upload-identity")]
  [EndpointName("UploadIdentity")]
  [EndpointSummary("Uploads identity documents for the current user")]
  [EndpointDescription("Uploads identity front and back images for the current user.")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> UploadIdentity([FromForm] UploadIdentityCommand command)
  {

    // var url = _storageClient.GetSasUrl("https://sakawader.blob.core.windows.net/identity-pics/303dd4a4-a97e-4629-88aa-be667f6f36c5.png", Containers.IdentityImages, TimeSpan.FromHours(1));
    // return Ok(url.Value);


    var uploadResult = await _sender.Send(command);
    return uploadResult.Match(
        _ => NoContent(),
        errors => Problem(errors));
  }



  [Consumes("multipart/form-data")]
  [HttpPut("update-profile-img")]
  [EndpointName("updateProileImage")]
  [EndpointSummary("update profile picture for current user")]
  [EndpointDescription("update profile  image for the current user.")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> UpdateProfilePicture([FromForm] UpdateProfileImageCommand command)
  {
    var uploadResult = await _sender.Send(command);
    return uploadResult.Match(
        _ => NoContent(),
        errors => Problem(errors));
  }



}


