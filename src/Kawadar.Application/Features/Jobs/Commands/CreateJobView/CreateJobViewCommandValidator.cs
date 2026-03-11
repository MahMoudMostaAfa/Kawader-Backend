using FluentValidation;

namespace Kawadar.Application.Features.Jobs.Commands.CreateJobView;

public class CreateJobViewCommandValidator : AbstractValidator<CreateJobViewCommand>
{
  public CreateJobViewCommandValidator()
  {
    RuleFor(x => x.Slug).NotEmpty().MaximumLength(100);
  }
}
