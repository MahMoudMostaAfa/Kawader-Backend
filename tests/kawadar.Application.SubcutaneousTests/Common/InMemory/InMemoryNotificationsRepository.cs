using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.ConversastionsAndMessages.EventHandlers;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Notifications;

namespace kawadar.Application.SubcutaneousTests.Common.InMemory;

public class InMemoryNotificationsRepository : INotificationsRepository
{
    private readonly Dictionary<Guid, Notification> _notifications = new();

    public Task<Result<Created>> AddNotificationAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        _notifications[notification.Id] = notification;
        return Task.FromResult<Result<Created>>(Result.Created);
    }

    public Task<Result<Notification>> GetByIdAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var found = _notifications.TryGetValue(notificationId, out var notification);
        return Task.FromResult(found
            ? (Result<Notification>)notification!
            : Error.NotFound("Notification.NotFound", "Notification not found."));
    }

    public Task<Result<IEnumerable<Notification>>> GetUserNotificationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        IEnumerable<Notification> list = _notifications.Values.Where(n => n.UserId == userId).ToList();
        return Task.FromResult<Result<IEnumerable<Notification>>>(list.ToList());
    }

    public Task<Result<PaginatedList<Notification>>> GetUserNotificationsAsync(Guid userId, int page, int pageSize)
    {
        var list = _notifications.Values.Where(n => n.UserId == userId).ToList();
        var paged = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult<Result<PaginatedList<Notification>>>(
            new PaginatedList<Notification>(paged, list.Count, page, pageSize));
    }
}
