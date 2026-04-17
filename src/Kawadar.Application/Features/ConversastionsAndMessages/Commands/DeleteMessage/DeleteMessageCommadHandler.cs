using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Conversations.Events;
using MediatR;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Commands.DeleteMessage;

public class DeleteMessageCommadHandler : IRequestHandler<DeleteMessageCommand, Result<MessageDto>>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IConversationsRepository _conversationsRepository;
  private readonly IUser _user;

  private readonly IUsersRepository _usersRepository;
  private readonly IIdentityService _identityService;


  public DeleteMessageCommadHandler(IUnitOfWork unitOfWork, IConversationsRepository conversationsRepository, IUser user, IUsersRepository usersRepository, IIdentityService identityService)
  {
    _unitOfWork = unitOfWork;
    _conversationsRepository = conversationsRepository;
    _user = user;
    _usersRepository = usersRepository;
    _identityService = identityService;
  }

  public async Task<Result<MessageDto>> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
  {
    string? userId = request.userId ?? _user.Id;
    if (userId == null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var messageResult = await _conversationsRepository.GetMessageByIdAsync(request.messageId);
    if (messageResult.IsError) return messageResult.Errors;
    var message = messageResult.Value;

    if (message.SenderUserId != userProfile.Id)
    {
      return ApplicationErrors.UnauthorizedAccess;
    }

    var deleteResult = message.Delete();
    if (deleteResult.IsError) return deleteResult.Errors;

    var identityResult = await _identityService.GetUserByIdAsync(userProfile.UserId);
    if (identityResult.IsError) return identityResult.Errors;
    var identityUser = identityResult.Value;
    var messageDto = new MessageDto
    {
      Id = message.Id,
      ConversationId = message.ConversationId,
      Content = message.Content,
      SentAt = message.CreatedAt,
      SenderUserName = identityUser.UserName,
    };

    message.AddDomainEvent(new DeletedMessageEvent(message.Id, message.ConversationId, userId, request.connectionId!, message.CreatedAt, userProfile.Id, message.Content));

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return messageDto;

  }
}