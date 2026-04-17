using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Notifications.Enums;
using MediatR;

namespace Kawadar.Domain.Notifications;

public class Notification : AuditableEntity
{
  public Guid UserId { get; private set; }
  public string Title { get; private set; } = default!;
  public string Body { get; private set; } = default!;

  public bool IsRead { get; private set; } = false;
  public DateTime? ReadAt { get; private set; }

  public NotificationCategory Category { get; private set; }
  public NotificationType Type { get; private set; }


  // ID of related entity (MessageId, JobId...)
  public Guid? ReferenceId { get; private set; }

  //"Conversation" | "Job" | "Payment"
  public string? ReferenceType { get; private set; }

  ///conversations/123"
  public string? RedirectUrl { get; private set; }
  private Notification()
  { }

  private Notification(Guid userId, string title, string body, NotificationCategory category, NotificationType type, Guid? referenceId = null, string? referenceType = null, string? redirectUrl = null) : base(Guid.NewGuid())
  {
    UserId = userId;
    Title = title;
    Body = body;
    Category = category;
    Type = type;
    ReferenceId = referenceId;
    ReferenceType = referenceType;
    RedirectUrl = redirectUrl;
  }

  public static Result<Notification> Create(Guid userId, string title, string body, NotificationCategory category, NotificationType type, Guid? referenceId = null, string? referenceType = null, string? redirectUrl = null)
  {
    return new Notification(userId, title, body, category, type, referenceId, referenceType, redirectUrl);
  }

  public Result<Updated> MarkAsRead()
  {
    if (IsRead) return Result.Updated; // Already read, no action needed

    IsRead = true;
    ReadAt = DateTime.UtcNow;
    return Result.Updated;
  }







}