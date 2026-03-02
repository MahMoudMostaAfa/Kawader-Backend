using FluentValidation;

namespace Kawadar.Application.Features.Jobs.Commands.DeleteJobAttachment;

public class DeleteJobAttachmentCommandValidator : AbstractValidator<DeleteJobAttachmentCommand>
{
  public DeleteJobAttachmentCommandValidator()
  {
    RuleFor(x => x.Slug).NotEmpty().MaximumLength(100);
    RuleFor(x => x.AttachmentId).NotEmpty();
  }
}
