using Kawadar.Domain.Notifications.Enums;

namespace Kawadar.Application.Features.ConversastionsAndMessages.DTOs;

public class NotificationDto
{
  public Guid Id { get; set; }
  public string Title { get; set; } = default!;
  public string Category { get; set; } = default!;
  public string Type { get; set; } = default!;
  public string? RedirectUrl { get; set; } = default!;
  public string Body { get; set; } = default!;
  public bool IsRead { get; set; }
  public DateTime ReceivedAt { get; set; }
}