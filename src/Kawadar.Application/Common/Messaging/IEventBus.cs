namespace Kawadar.Application.Common.Messaging;

public interface IEventBus
{

  Task PublishAsync<T>(T message, CancellationToken ct = default) where T : class;

  Task SendAsync<T>(T message, string queueName, CancellationToken ct = default) where T : class;


}