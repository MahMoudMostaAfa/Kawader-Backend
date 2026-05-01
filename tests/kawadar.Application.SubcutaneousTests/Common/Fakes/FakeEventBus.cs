using Kawadar.Application.Common.Messaging;

namespace kawadar.Application.SubcutaneousTests.Common.Fakes;

public class FakeEventBus : IEventBus
{
    public readonly List<object> PublishedMessages = [];

    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        PublishedMessages.Add(message);
        return Task.CompletedTask;
    }

    public Task SendAsync<T>(T message, string queueName, CancellationToken ct = default) where T : class
    {
        PublishedMessages.Add(message);
        return Task.CompletedTask;
    }

    public void Clear()
    {
        PublishedMessages.Clear();
    }
}
