using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Queries.GetMyConversations;

public class GetMyConversationsQueryHandler : IRequestHandler<GetMyConversationsQuery, Result<PaginatedList<ConversationDto>>>
{

  private readonly IConversationsRepository _conversationsRepository;

  private readonly IIdentityService _identityService;

  private readonly IUsersRepository _usersRepository;
  private readonly IUser _user;
  public GetMyConversationsQueryHandler(IConversationsRepository conversationsRepository, IIdentityService identityService, IUsersRepository usersRepository, IUser user)
  {
    _conversationsRepository = conversationsRepository;
    _identityService = identityService;
    _usersRepository = usersRepository;
    _user = user;

  }
  public async Task<Result<PaginatedList<ConversationDto>>> Handle(GetMyConversationsQuery request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;

    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var currentUserProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (currentUserProfileResult.IsError) return currentUserProfileResult.Errors;
    var currentUserProfile = currentUserProfileResult.Value;

    var conversationsResult = await _conversationsRepository.GetConversationsForUserAsync(currentUserProfile.Id, request.pageNumber, request.pageSize, cancellationToken);
    if (conversationsResult.IsError) return conversationsResult.Errors;
    var conversations = conversationsResult.Value;

    var validConversationDtos = new List<ConversationDto>();

    foreach (var c in conversations.Items)
    {
      var otherUserProfileId = c.SenderUserId == currentUserProfile.Id ? c.ReceiverUserId : c.SenderUserId;
      var otherUserProfileResult = await _usersRepository.GetUserProfileByIdAsync(otherUserProfileId);

      if (otherUserProfileResult.IsError) continue;
      var otherUserProfile = otherUserProfileResult.Value;
      var userIdentiyResult = await _identityService.GetUserByIdAsync(otherUserProfile.UserId);
      if (userIdentiyResult.IsError) continue;
      var userIdentity = userIdentiyResult.Value;

      validConversationDtos.Add(new ConversationDto
      {
        Id = c.Id,
        IsLastMessageByCurrentUser = c.LastMessage != null && c.LastMessage.SenderUserId == currentUserProfile.Id,
        LastMessageContent = c.LastMessage != null ? c.LastMessage.Content : string.Empty,
        LastMessageSentAt = c.LastMessage != null ? c.LastMessage.CreatedAt : c.CreatedAt,
        Title = c.Title,
        OtherParticipantFullName = otherUserProfile.FullName,
        OtherParticipantUserName = userIdentity.UserName,
        otherParticipantProfilePictureUrl = otherUserProfile.ProfilePictureUrl ?? string.Empty
      });
    }

    var paginatedList = new PaginatedList<ConversationDto>(validConversationDtos, conversations.TotalCount, conversations.PageNumber, request.pageSize);

    return paginatedList;
  }
}