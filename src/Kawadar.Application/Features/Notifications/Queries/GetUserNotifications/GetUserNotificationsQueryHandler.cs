using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Application.Features.ConversastionsAndMessages.EventHandlers;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Notifications.Queries.GetUserNotifications;

public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, Result<PaginatedList<NotificationDto>>>
{

  private readonly IMapper _mapper;
  private readonly INotificationsRepository _notificationRepository;
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;

  public GetUserNotificationsQueryHandler(INotificationsRepository notificationRepository, IUser user, IUsersRepository usersRepository, IMapper mapper)
  {
    _notificationRepository = notificationRepository;
    _user = user;
    _usersRepository = usersRepository;
    _mapper = mapper;
  }

  public async Task<Result<PaginatedList<NotificationDto>>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;

    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;


    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);

    if (userProfileResult.IsError) return userProfileResult.Errors;

    var userProfile = userProfileResult.Value;
    var notificationsResult = await _notificationRepository.GetUserNotificationsAsync(userProfile.Id, request.Page, request.PageSize);

    if (notificationsResult.IsError) return notificationsResult.Errors;

    var notifications = notificationsResult.Value;

    var notificationDtos = notifications.Items.Select(n => _mapper.Map<NotificationDto>(n)).ToList();

    return new PaginatedList<NotificationDto>(notificationDtos, notifications.TotalCount, notifications.PageNumber, request.PageSize);



  }
}