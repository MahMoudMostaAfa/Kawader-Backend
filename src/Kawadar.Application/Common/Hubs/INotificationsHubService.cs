using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;

namespace Kawadar.Application.Common.Hubs;


public interface INotificationsHubService
{
  Task SendNotificationAsync(string userId, NotificationDto notificationDto, CancellationToken cancellationToken = default);
}