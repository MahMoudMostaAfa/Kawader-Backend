using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Commands.DeleteConversation;

public class DeleteConversationCommandHandler : IRequestHandler<DeleteConversationCommand, Result<Deleted>>
{
  private readonly IConversationsRepository _conversationsRepository;
  private readonly IUnitOfWork _unitOfWork;

  private readonly IUsersRepository _usersRepository;
  private readonly IUser _user;

  public DeleteConversationCommandHandler(IConversationsRepository conversationsRepository, IUnitOfWork unitOfWork, IUsersRepository usersRepository, IUser user)
  {
    _conversationsRepository = conversationsRepository;
    _unitOfWork = unitOfWork;
    _usersRepository = usersRepository;
    _user = user;

  }


  public async Task<Result<Deleted>> Handle(DeleteConversationCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;
    var conversationResult = await _conversationsRepository.GetConversationByIdAsync(request.ConversationId, cancellationToken);
    if (conversationResult.IsError) return conversationResult.Errors;
    var conversation = conversationResult.Value;

    if (conversation.SenderUserId != userProfile.Id) return ApplicationErrors.UnauthorizedAccess;

    var isOtherUserJoinedResult = await _conversationsRepository.IsOhterUserJoinedConversationAsync(conversation.Id, userProfile.Id, cancellationToken);
    if (isOtherUserJoinedResult.IsError) return isOtherUserJoinedResult.Errors;
    if (isOtherUserJoinedResult.Value) return Error.Validation("Conversation.HasMessagesFromOtherUser", "Cannot delete conversation because the other user has already joined the conversation");

    if (conversation.LastMessageId is not null)
    {
      var clearLastMessageResult = conversation.ClearLastMessageReference();
      if (clearLastMessageResult.IsError) return clearLastMessageResult.Errors;

      await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    var deleteResult = _conversationsRepository.DeleteConversationAsync(conversation, cancellationToken);
    if (deleteResult.IsError) return deleteResult.Errors;

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Deleted;

  }
}
