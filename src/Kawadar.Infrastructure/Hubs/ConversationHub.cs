using Kawadar.Application.Common.Hubs;
using Kawadar.Application.Features.ConversastionsAndMessages.Commands.SendMessage;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Infrastructure.Services.HubServices.SignalRDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Infrastructure.Hubs;

[Authorize]
public class ConversationHub : Hub
{
  private readonly IConversationsHubService _conversationsHubService;
  private readonly ILogger<ConversationHub> _logger;
  private readonly ISender _sender;
  public ConversationHub(IConversationsHubService conversationsHubService, ILogger<ConversationHub> logger, ISender sender)
  {
    _conversationsHubService = conversationsHubService;
    _logger = logger;
    _sender = sender;
  }

  public override Task OnConnectedAsync()
  {
    _logger.LogInformation("User connected to ConversationHub. ConnectionId: {ConnectionId}, UserId: {UserId}", Context.ConnectionId, Context.UserIdentifier);

    return base.OnConnectedAsync();
  }

  public override Task OnDisconnectedAsync(Exception? exception)
  {
    _logger.LogInformation("User disconnected from ConversationHub. ConnectionId: {ConnectionId}, UserId: {UserId}, Exception: {Exception}", Context.ConnectionId, Context.UserIdentifier, exception?.Message);
    return base.OnDisconnectedAsync(exception);
  }

  public async Task JoinConversation(Guid conversationId)
  {
    // Verify that user is a participant of the conversation before allowing them to join the group
    var userId = Context.UserIdentifier;
    if (userId == null)
    {
      throw new HubException("User is not authenticated.");
    }
    var isExistResult = await _conversationsHubService.IsUserInConversationAsync(conversationId, userId);
    if (isExistResult.IsError) throw new HubException(isExistResult.Errors[0].Description);

    if (!isExistResult.Value) throw new HubException("User is not a participant of the conversation.");
    _logger.LogInformation("User {UserId} is joining conversation {ConversationId}", userId, conversationId);
    await Groups.AddToGroupAsync(Context.ConnectionId, ConversationGroup(conversationId));
  }


  public async Task LeaveConversation(Guid conversationId)
  {
    // Verify that user is a participant of the conversation before allowing them to join the group

    var userId = Context.UserIdentifier;
    if (userId == null)
    {
      throw new HubException("User is not authenticated.");
    }
    var isExistResult = await _conversationsHubService.IsUserInConversationAsync(conversationId, userId);
    if (isExistResult.IsError) throw new HubException(isExistResult.Errors[0].Description);

    if (!isExistResult.Value) throw new HubException("User is not a participant of the conversation.");
    _logger.LogInformation("User {UserId} is leaving conversation {ConversationId}", userId, conversationId);
    await Groups.RemoveFromGroupAsync(Context.ConnectionId, ConversationGroup(conversationId));
  }

  public async Task Typing(Guid conversationId, bool isTyping)
  {
    var userId = Context.UserIdentifier;
    if (userId == null)
    {
      throw new HubException("User is not authenticated.");
    }
    await Clients.GroupExcept(ConversationGroup(conversationId), Context.ConnectionId).SendAsync("TypingIndicator", new TypingIndicator(conversationId, userId, isTyping));

  }


  //<summary>
  // this method to send text messages only throw the hub, for messages with attachments use the SendMessage method which will handle both text and attachments
  //</summary>
  public async Task<MessageDto> SendMessage(SendSignalRMessageRequest sendMessageRequest)
  {
    var userId = Context.UserIdentifier;
    if (userId == null)
    {
      throw new HubException("User is not authenticated.");
    }
    var sendMessageResult = await _sender.Send(
      new SendMessageCommand(userId, Context.ConnectionId, sendMessageRequest.ConversationId, sendMessageRequest.Content, sendMessageRequest.ReplyToMessageId, null, null)
    );
    if (sendMessageResult.IsError)
    {
      var error = sendMessageResult.Errors[0];
      _logger.LogError("Error sending message: {ErrorDescription}", error.Description);
      throw new HubException(error.Description);
    }

    var message = sendMessageResult.Value;
    _logger.LogInformation("User {UserId} sent a message to conversation {ConversationId}. MessageId: {MessageId}", userId, sendMessageRequest.ConversationId, message.Id);
    return message;
  }
  public static string ConversationGroup(Guid conversationId) => $"conversation:{conversationId}";
}