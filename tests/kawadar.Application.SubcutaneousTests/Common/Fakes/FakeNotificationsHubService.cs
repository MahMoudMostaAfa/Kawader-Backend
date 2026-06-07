using Kawadar.Application.Common.Hubs;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;

namespace kawadar.Application.SubcutaneousTests.Common.Fakes;

public class FakeNotificationsHubService : INotificationsHubService
{
    public Task SendNotificationAsync(string userId, NotificationDto notificationDto, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task SendNotificationToAllAsync(NotificationDto notificationDto, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
