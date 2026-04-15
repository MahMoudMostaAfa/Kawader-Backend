using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Notifications;

namespace Kawadar.Application.Features.ConversastionsAndMessages.EventHandlers;


public interface INotificationsRepository
{
  Task<Result<Created>> AddNotificationAsync(Notification notification, CancellationToken cancellationToken = default);
}