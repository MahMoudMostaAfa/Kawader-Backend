using Kawadar.Domain.Common;

namespace Kawadar.Domain.Jobs.Events;

public class JobCreatedEvent : DomainEvent
{
  public Job Job { get; set; } = null!;
}