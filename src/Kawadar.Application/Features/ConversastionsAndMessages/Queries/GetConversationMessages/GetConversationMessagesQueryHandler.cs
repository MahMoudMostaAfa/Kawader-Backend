using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Domain.Common.Constants;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles.Enums;
using MediatR;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Queries.GetConversationMessages;

public class GetConversationMessagesQueryHandler : IRequestHandler<GetConversationMessagesQuery, Result<ConversationMessagesDto>>
{
  private readonly IConversationsRepository _conversationsRepository;

  private readonly IUsersRepository _usersRepository;
  private readonly IUser _user;
  private readonly IIdentityService _identityService;

  public GetConversationMessagesQueryHandler(IConversationsRepository conversationsRepository, IUsersRepository usersRepository, IUser user, IIdentityService identityService)
  {
    _conversationsRepository = conversationsRepository;
    _usersRepository = usersRepository;
    _user = user;
    _identityService = identityService;




  }


  public async Task<Result<ConversationMessagesDto>> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var conversationResult = await _conversationsRepository.GetConversationByIdAsync(request.conversationId);
    if (conversationResult.IsError) return conversationResult.Errors;
    var conversation = conversationResult.Value;

    var userClaimsResult = await _identityService.GetUserClaimsAsync(userId);
    if (userClaimsResult.IsError) return userClaimsResult.Errors;

    var hasAccessToConversation = userClaimsResult.Value.Select(x => x.Value).Contains(Permissions.ViewConversations);
    var proposalId = conversation.ProposalId;
    var jobId = conversation.JobId ?? conversation.Proposal?.JobId;


    // CHECK THAT USER IS A PARTICIPANT IN THE CONVERSATION
    if (!hasAccessToConversation)
    {
        if((conversation.ReceiverUserId != userProfile.Id && conversation.SenderUserId != userProfile.Id))
            return ApplicationErrors.UnauthorizedAccess;
    }
    // get messages for the conversation
    var messagesResult = await _conversationsRepository.GetMessagesForConversationAsync(request.conversationId, request.PageNumber, request.PageSize, cancellationToken);
    if (messagesResult.IsError) return messagesResult.Errors;

    var messages = messagesResult.Value;


    List<MessageDto> messageDtos = new List<MessageDto>();
    foreach (var message in messages.Items)
    {
      var senderUserResult = await _usersRepository.GetUserProfileByIdAsync(message.SenderUserId);
      if (senderUserResult.IsError) return senderUserResult.Errors;
      var senderUser = senderUserResult.Value;
      var senderIdentityResult = await _identityService.GetUserByIdAsync(senderUser.UserId);
      if (senderIdentityResult.IsError) return senderIdentityResult.Errors;
      var senderIdentity = senderIdentityResult.Value;

      var messageDto = new MessageDto
      {
        Id = message.Id,
        ConversationId = message.ConversationId,
        SenderUserName = senderIdentity.UserName,
        Content = message.Content,
        SentAt = message.CreatedAt,
        messageReplyDto = message.ReplayToMessageId != null ? new MessageReplyDto
        {
          Id = message.ReplayToMessageId ?? Guid.Empty,
          Content = message.ReplayToMessage != null ? message.ReplayToMessage.Content : string.Empty
        } : null,
        Attachments = message.Files.Select(a => new MessageAttachmentDto
        {
          Id = a.Id,
          FileUrl = a.File.FileUrl,
          FileName = a.File.FileName,
          ContentType = a.File.MimeType,
          FileSizeInBytes = a.File.FileSizeInBytes
        }).ToList()
      };
      messageDtos.Add(messageDto);
    }


    var paginatedList = new PaginatedList<MessageDto>(messageDtos, messages.TotalCount, request.PageNumber, request.PageSize);
    return new ConversationMessagesDto
    {
      ProposalId = proposalId,
      JobId = jobId,
      Messages = paginatedList
    };
  }
}
