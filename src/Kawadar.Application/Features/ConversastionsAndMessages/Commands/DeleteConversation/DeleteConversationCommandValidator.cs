using FluentValidation;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Commands.DeleteConversation;

public class DeleteConversationCommandValidator : AbstractValidator<DeleteConversationCommand>
{
  public DeleteConversationCommandValidator()
  {
    RuleFor(x => x.ConversationId).NotEmpty().WithMessage("Conversation ID is required.");
  }
}