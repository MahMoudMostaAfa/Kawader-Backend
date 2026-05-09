
using Kawadar.Api.Requests.Conversation;
using Kawadar.Application.Features.ConversastionsAndMessages.Commands.CreateConversation;
using Kawadar.Application.Features.ConversastionsAndMessages.Commands.DeleteConversation;
using Kawadar.Application.Features.ConversastionsAndMessages.Commands.SendMessage;
using Kawadar.Application.Features.ConversastionsAndMessages.Queries.GetConversationMessages;
using Kawadar.Application.Features.ConversastionsAndMessages.Queries.GetMyConversations;
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

    var command = new CreateConversationCommand(request.ReceiverUserName, request.ProposalId, request.Title, request.InitialMessageContent);

    var result = await _sender.Send(command);

    return result.Match(
       _ => Created(),
       errors => Problem(errors)
     );
  }


  // Implement the logic to retrieve all conversations for the authenticated user
  [HttpGet]
  public async Task<IActionResult> GetConversations([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
  {
    var query = new GetMyConversationsQuery(pageNumber, pageSize);

    var result = await _sender.Send(query);
    return result.Match(
      paginatedConversations => Ok(paginatedConversations),
      errors => Problem(errors)
     );

  }


  [HttpDelete("{conversationId:guid}")]
  public async Task<IActionResult> DeleteConversation(Guid conversationId)
  {
    var command = new DeleteConversationCommand(conversationId);

    var result = await _sender.Send(command);
    return result.Match(
      deleted => NoContent(),
      errors => Problem(errors)
     );
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

  [HttpGet("{conversationId:guid}/messages")]
  public async Task<IActionResult> GetConversationMessages(Guid conversationId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
  {
    var query = new GetConversationMessagesQuery(conversationId, pageNumber, pageSize);
    var result = await _sender.Send(query);
    return result.Match(
      paginatedMessages => Ok(paginatedMessages),
      errors => Problem(errors)
    );
  }
}