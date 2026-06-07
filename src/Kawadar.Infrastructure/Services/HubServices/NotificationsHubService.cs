using Kawadar.Application.Common.Hubs;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Infrastructure.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Kawadar.Infrastructure.Services.HubServices;


public class NotificationsHubService : INotificationsHubService
{
  private readonly IHubContext<NotificationHub> _hubContext;

  public NotificationsHubService(IHubContext<NotificationHub> hubContext)
  {
    _hubContext = hubContext;
  }
  public Task SendNotificationAsync(string userId, NotificationDto notificationDto, CancellationToken cancellationToken = default)
  {
    return _hubContext.Clients.Group(NotificationHub.UserGroup(userId)).SendAsync("ReceiveNotification", notificationDto, cancellationToken);
  }

  public Task SendNotificationToAllAsync(NotificationDto notificationDto, CancellationToken cancellationToken = default)
  {
    return _hubContext.Clients.All.SendAsync("ReceiveNotification", notificationDto, cancellationToken);
  }
}
