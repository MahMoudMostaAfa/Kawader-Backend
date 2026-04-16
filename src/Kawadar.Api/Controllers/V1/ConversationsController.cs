
using Kawadar.Api.Requests.Conversation;
using Kawadar.Application.Features.ConversastionsAndMessages.Commands.CreateConversation;
using Kawadar.Application.Features.ConversastionsAndMessages.Commands.SendMessage;
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
       conversationId => CreatedAtAction(nameof(GetConversationById), new { conversationId }, conversationId),
       errors => Problem(errors)
     );
  }


  [HttpGet("{conversationId:guid}")]
  public IActionResult GetConversationById(Guid conversationId)
  {
    // Implement the logic to retrieve a conversation by its ID
    return Ok();
  }

  [HttpPost("{conversationId:guid}/messages")]
  public async Task<IActionResult> SendMessage(Guid conversationId, [FromForm] SendMessageRequest request)
  {

    var command = new SendMessageCommand(null, request.ConnectionId, conversationId, request.Content, request.ReplyToMessageId, request.AttachmentFiles, request.AttachmentLinks);
    var result = await _sender.Send(command);
    // Implement the logic to send a message in a conversation
    return result.Match(
      messageDto => Ok(messageDto),
      errors => Problem(errors)
     );

  }


}