using Kawadar.Application.Common.Hubs;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Conversations.Messages;
using Kawadar.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Kawadar.Infrastructure.Services.HubServices;

public class ConversationsHubService : IConversationsHubService
{
  private readonly IConversationsRepository _conversationsRepository;
  private readonly IUsersRepository _usersRepository;
  private readonly IHubContext<ConversationHub> _hubContext;


  public ConversationsHubService(IConversationsRepository conversationsRepository, IUsersRepository usersRepository, IHubContext<ConversationHub> hubContext)
  {
    _conversationsRepository = conversationsRepository;
    _usersRepository = usersRepository;
    _hubContext = hubContext;

  }
  public async Task<Result<bool>> IsUserInConversationAsync(Guid conversationId, string userId)
  {
    // 1- get thhe user Profile 
    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;
    // 2- get the conversation 

    var conversationResult = await _conversationsRepository.GetConversationByIdAsync(conversationId);
    if (conversationResult.IsError) return conversationResult.Errors;
    var conversation = conversationResult.Value;
    // 3- check if the user is a participant in the conversation

    if (userProfile.Id != conversation.ReceiverUserId && userProfile.Id != conversation.SenderUserId) return false;

    return true;
  }

  public Task SendDeletedMessageToConversationAsync(Guid conversationId, string? connectionId, string recipientId, MessageDto message)
  {
    return _hubContext.Clients.GroupExcept(ConversationHub.ConversationGroup(conversationId), connectionId!).SendAsync("ReceiveDeletedMessage", message);
  }

  public Task SendEditedMessageToConversationAsync(Guid conversationId, string? connectionId, string recipientId, MessageDto message)
  {
    return _hubContext.Clients.GroupExcept(ConversationHub.ConversationGroup(conversationId), connectionId!).SendAsync("ReceiveEditedMessage", message);
  }

  public Task SendMessageToConversationAsync(Guid conversationId, string? connectionId, string recipientId, MessageDto message)
  {


    if (connectionId is not null) return _hubContext.Clients.GroupExcept(ConversationHub.ConversationGroup(conversationId), connectionId).SendAsync("ReceiveMessage", message);

    else return _hubContext.Clients.Group(ConversationHub.ConversationGroup(conversationId)).SendAsync("ReceiveMessage", message);
  }



}