using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.ConversastionsAndMessages.EventHandlers;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Notifications.Commands.MarkNotifiactionAsRead;

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotifiationAsReadCommand, Result<Updated>>
{
  private readonly INotificationsRepository _notificationsRepository;
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IUnitOfWork _unitOfWork;

  public MarkNotificationAsReadCommandHandler(INotificationsRepository notificationsRepository, IUser user, IUsersRepository usersRepository, IUnitOfWork unitOfWork)
  {
    _notificationsRepository = notificationsRepository;
    _user = user;
    _usersRepository = usersRepository;
    _unitOfWork = unitOfWork;
  }
  public async Task<Result<Updated>> Handle(MarkNotifiationAsReadCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;


    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var notificationResult = await _notificationsRepository.GetByIdAsync(request.NotificationId);
    if (notificationResult.IsError) return notificationResult.Errors;
    var notification = notificationResult.Value;
    if (notification.UserId != userProfile.Id) return ApplicationErrors.UnauthorizedAccess;

    var udaptedResult = notification.MarkAsRead();
    if (udaptedResult.IsError) return udaptedResult.Errors;

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Updated;
  }
}