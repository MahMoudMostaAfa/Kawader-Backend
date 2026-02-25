using Kawadar.Application.Common.Messaging;
using MassTransit;

namespace Kawadar.Infrastructure.Messaging;

public class EventBus : IEventBus
{
  private readonly IPublishEndpoint _publishEndpoint;
  private readonly ISendEndpointProvider _sendEndpointProvider;

  public EventBus(IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider)
  {
    _publishEndpoint = publishEndpoint;
    _sendEndpointProvider = sendEndpointProvider;

  }

  // publish = fanout style , mulitple consumer can react 
  public async Task PublishAsync<T>(T message, CancellationToken ct = default) where T : class
  {
    await _publishEndpoint.Publish(message, ct);
  }


  // send = direct to specific queue
  public async Task SendAsync<T>(T message, string queueName, CancellationToken ct = default) where T : class
  {
    var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queueName}"));
    await endpoint.Send(message, ct);

  }
}