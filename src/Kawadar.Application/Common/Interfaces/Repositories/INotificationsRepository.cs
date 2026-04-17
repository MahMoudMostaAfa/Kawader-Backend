using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Notifications;

namespace Kawadar.Application.Features.ConversastionsAndMessages.EventHandlers;


public interface INotificationsRepository
{
  Task<Result<Created>> AddNotificationAsync(Notification notification, CancellationToken cancellationToken = default);

  Task<Result<Notification>> GetByIdAsync(Guid notificationId, CancellationToken cancellationToken = default);

  Task<Result<IEnumerable<Notification>>> GetUserNotificationsAsync(Guid userId, CancellationToken cancellationToken = default);

  Task<Result<PaginatedList<Notification>>> GetUserNotificationsAsync(Guid userId, int page, int pageSize);
}