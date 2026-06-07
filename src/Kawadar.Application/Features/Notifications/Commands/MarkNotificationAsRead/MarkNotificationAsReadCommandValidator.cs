using FluentValidation;

namespace Kawadar.Application.Features.Notifications.Commands.MarkNotifiactionAsRead;

public class MarkNotificationAsReadCommandValidator : AbstractValidator<MarkNotifiationAsReadCommand>
{
  public MarkNotificationAsReadCommandValidator()
  {
    RuleFor(x => x.NotificationId).NotEmpty().WithMessage("Notification ID is required.");
  }
}