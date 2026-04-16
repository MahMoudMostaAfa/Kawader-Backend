namespace Kawadar.Application.Features.Notifications.DTOs;


public class NotificationDto
{
  public Guid Id { get; set; }
  public string Message { get; set; } = null!;
  public string Type { get; set; } = null!;
  public string Category { get; set; } = null!;
  public string? RedirectUrl { get; set; }
  public bool IsRead { get; set; }
  public DateTime CreatedAt { get; set; }
}