using FluentValidation;

namespace Kawadar.Application.Features.Jobs.Commands.DeleteJob;

public class DeleteJobCommandValidator : AbstractValidator<DeleteJobCommand>
{
  public DeleteJobCommandValidator()
  {
    RuleFor(x => x.Slug).NotEmpty().MaximumLength(100);
  }
}
