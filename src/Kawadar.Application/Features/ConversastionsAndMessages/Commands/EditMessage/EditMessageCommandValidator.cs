using FluentValidation;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Commands.EditMessage;

public class EditMessageCommandValidator : AbstractValidator<EditMessageCommand>
{
  public EditMessageCommandValidator()
  {
    When(x => x.connectionId != null, () =>
    {
      RuleFor(x => x.connectionId).NotEmpty().WithMessage("ConnectionId cannot be empty when provided.");
    });
    When(x => x.userId != null, () =>
    {
      RuleFor(x => x.userId).NotEmpty().WithMessage("UserId cannot be empty when provided.");
    });
    RuleFor(x => x.messageId).NotEqual(Guid.Empty).WithMessage("MessageId cannot be empty.");
    RuleFor(x => x.newContent).NotEmpty().WithMessage("NewContent cannot be empty.");

  }
}