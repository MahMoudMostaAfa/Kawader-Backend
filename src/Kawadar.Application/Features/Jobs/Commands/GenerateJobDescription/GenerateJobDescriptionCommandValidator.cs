using FluentValidation;

namespace Kawadar.Application.Features.Jobs.Commands.GenerateJobDescription;

public class GenerateJobDescriptionCommandValidator : AbstractValidator<GenerateJobDescriptionCommand>
{
  public GenerateJobDescriptionCommandValidator()
  {
    RuleFor(x => x.Context)
        .NotEmpty().WithMessage("Context is required to generate a job description.")
        .MinimumLength(10).WithMessage("Context must be at least 10 characters long.")
        .MaximumLength(2000).WithMessage("Context must not exceed 2000 characters.");
  }
}
