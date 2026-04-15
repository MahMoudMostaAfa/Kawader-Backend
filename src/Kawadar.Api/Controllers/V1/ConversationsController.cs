
using Kawadar.Api.Requests.Conversation;
using Kawadar.Application.Features.ConversastionsAndMessages.Commands.CreateConversation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/conversations")]
[Authorize]
public class ConversationsController : ApiController
{

  private readonly ISender _sender;

  public ConversationsController(ISender sender)
  {
    _sender = sender;
  }
  [HttpPost]
  public async Task<IActionResult> CreateConversation([FromBody] CreateConversationRequest request)
  {

    var command = new CreateConversationCommand(request.ReceiverUserName, request.JobId, request.Title, request.InitialMessageContent);

    var result = await _sender.Send(command);

    return result.Match(
       conversationId => CreatedAtAction(nameof(GetConversationById), new { id = conversationId }, conversationId),
       errors => Problem(errors)
     );
  }


  [HttpGet("{id}")]
  public IActionResult GetConversationById(Guid id)
  {
    // Implement the logic to retrieve a conversation by its ID
    return Ok();
  }
}