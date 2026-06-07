using FluentValidation;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Commands.DeleteMessage;

public class DeleteMessageCommandValidator : AbstractValidator<DeleteMessageCommand>
{
  public DeleteMessageCommandValidator()
  {
    When(x => x.userId != null, () =>
    {
      RuleFor(x => x.userId).NotEmpty().WithMessage("User Id is required.");
    });
    When(x => x.connectionId != null, () =>
    {
      RuleFor(x => x.connectionId).NotEmpty().WithMessage("Connection Id is required.");
    });


    RuleFor(x => x.messageId).NotEmpty().WithMessage("Message Id is required.");

  }
}