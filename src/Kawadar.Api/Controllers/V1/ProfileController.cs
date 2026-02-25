using Google.GenAI.Types;
using Kawadar.Api.Requests.PortfolioProject.PortfolioItem;
using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Messaging;
using Kawadar.Application.Common.Messaging.Messages;
using Kawadar.Application.Features.Auth.Commands.Register;
using Kawadar.Application.Features.ProfileManagment.Commands.UpdateProfile;
using Kawadar.Application.Features.ProfileManagment.Commands.UploadIdentity;
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
  private readonly IStorageClient _storageClient;
  private readonly IAIService _aiService;
  private readonly IEventBus _eventBus;
  public ProfileController(ISender sender, IStorageClient storageClient, IAIService aiService, IEventBus eventBus)
  {
    _sender = sender;
    _storageClient = storageClient;
    _aiService = aiService;
    _eventBus = eventBus;

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



  [Consumes("multipart/form-data")]
  [HttpPost("upload-identity")]
  [EndpointName("UploadIdentity")]
  [EndpointSummary("Uploads identity documents for the current user")]
  [EndpointDescription("Uploads identity front and back images for the current user.")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> UploadIdentity([FromForm] UploadIdentityCommand command)
  {

    // var url = _storageClient.GetSasUrl("https://sakawader.blob.core.windows.net/identity-pics/303dd4a4-a97e-4629-88aa-be667f6f36c5.png", Containers.IdentityImages, TimeSpan.FromHours(1));
    // return Ok(url.Value);


    var uploadResult = await _sender.Send(command);
    return uploadResult.Match(
        _ => NoContent(),
        errors => Problem(errors));
  }

  // test endpoint for AI service
  [HttpPost("test-ai")]
  public async Task<IActionResult> TestAI([FromQuery] string prompt)
  {
    var schema = new Schema
    {
      Type = Google.GenAI.Types.Type.Object,
      Properties = new System.Collections.Generic.Dictionary<string, Schema>
      {
        { "message", new Schema { Type = Google.GenAI.Types.Type.String } }
      },
      Required = new List<string> { "message" }
    };

    var result = await _aiService.GenrateStructuredResponseAsync<TestAIResponse>(prompt, schema);

    return result.Match(
        response => Ok(response),
        errors => Problem(errors));
  }

  [HttpPost("test-rabbit")]
  public async Task<IActionResult> TestRabbit(RegisterCommand registerCommand)
  {

    var message = new SendWelcomeEmailMessage(Email: registerCommand.Email, FullName: registerCommand.FirstName);
    await _eventBus.PublishAsync<SendWelcomeEmailMessage>(message);

    return Ok("registered");
  }
}


internal class TestAIResponse
{
  public string Message { get; set; } = string.Empty;
}