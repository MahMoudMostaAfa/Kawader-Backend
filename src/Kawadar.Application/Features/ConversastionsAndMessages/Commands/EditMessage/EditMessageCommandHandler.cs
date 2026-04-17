using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Conversations.Events;
using Kawadar.Domain.Conversations.Messages;
using MediatR;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Commands.EditMessage;

public class EditMessageCommandHandler : IRequestHandler<EditMessageCommand,
Result<MessageDto>>
{
  private readonly IConversationsRepository _coversationsRepository;
  private readonly IUnitOfWork _unitOfWork;
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IIdentityService _identityService;
  public EditMessageCommandHandler(IConversationsRepository conversationsRepository, IUnitOfWork unitOfWork, IUser user, IUsersRepository usersRepository, IIdentityService identityService)
  {
    _coversationsRepository = conversationsRepository;
    _unitOfWork = unitOfWork;
    _user = user;
    _usersRepository = usersRepository;
    _identityService = identityService;

  }


  public async Task<Result<MessageDto>> Handle(EditMessageCommand request, CancellationToken cancellationToken)
  {
    string? userId = request.userId;
    if (userId is null) userId = _user.Id;

    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;
    var messageResult = await _coversationsRepository.GetMessageByIdAsync(request.messageId);
    if (messageResult.IsError) return messageResult.Errors;
    var message = messageResult.Value;
    if (message.SenderUserId != userProfile.Id) return ApplicationErrors.UnauthorizedAccess;
    var editResult = message.UpdateContent(request.newContent);
    if (editResult.IsError) return editResult.Errors;

    var identityResult = await _identityService.GetUserByIdAsync(userProfile.UserId);
    if (identityResult.IsError) return identityResult.Errors;
    var identityUser = identityResult.Value;
    var messageDto = new MessageDto
    {
      Id = message.Id,
      Content = message.Content,
      SenderUserName = identityUser.UserName,
      SentAt = message.CreatedAt,
      ConversationId = message.ConversationId,

    };

    message.AddDomainEvent(new EditedMessageEvent(message.Id, message.ConversationId, userId, request.connectionId!, message.CreatedAt, userProfile.Id, request.newContent));

    await _unitOfWork.SaveChangesAsync(cancellationToken);






    return messageDto;
  }
}