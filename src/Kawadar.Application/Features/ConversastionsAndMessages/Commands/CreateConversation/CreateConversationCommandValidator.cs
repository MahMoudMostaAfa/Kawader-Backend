using FluentValidation;
using MediatR;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Commands.CreateConversation;


public class CreateConversationCommandValidator : AbstractValidator<CreateConversationCommand>
{
  public CreateConversationCommandValidator()
  {
    RuleFor(c => c.ReceiverUserName).NotEmpty().WithMessage("Receiver user name is required.");
    RuleFor(c => c.ProposalId).NotEmpty().WithMessage("Proposal id is required.");
    RuleFor(c => c.Title).NotEmpty().WithMessage("Conversation title is required.")
        .MaximumLength(100).WithMessage("Conversation title must not exceed 100 characters.");
    RuleFor(c => c.InitialMessageContent).NotEmpty().WithMessage("Initial message content is required.")
        .MaximumLength(1000).WithMessage("Initial message content must not exceed 1000 characters.");
  }
}