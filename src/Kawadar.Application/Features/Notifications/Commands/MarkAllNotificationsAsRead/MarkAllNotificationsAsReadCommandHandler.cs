using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.ConversastionsAndMessages.EventHandlers;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead;

public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, Result<Updated>>
{
  private readonly INotificationsRepository _notificationsRepository;

  private readonly IUsersRepository _usersRepository;
  private readonly IUnitOfWork _unitOfWork;
  private readonly IUser _user;

  public MarkAllNotificationsAsReadCommandHandler(INotificationsRepository notificationsRepository, IUnitOfWork unitOfWork, IUser user, IUsersRepository usersRepository)
  {
    _notificationsRepository = notificationsRepository;
    _unitOfWork = unitOfWork;
    _user = user;
    _usersRepository = usersRepository;
  }
  public async Task<Result<Updated>> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;
    var notificationsResult = await _notificationsRepository.GetUserNotificationsAsync(userProfile.Id, cancellationToken);
    if (notificationsResult.IsError) return notificationsResult.Errors;
    var notifications = notificationsResult.Value;

    foreach (var notification in notifications)
    {
      notification.MarkAsRead();

    }
    await _unitOfWork.SaveChangesAsync(cancellationToken);
    return Result.Updated;



  }
}