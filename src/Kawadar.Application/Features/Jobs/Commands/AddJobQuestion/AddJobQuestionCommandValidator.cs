using FluentValidation;

namespace Kawadar.Application.Features.Jobs.Commands.AddJobQuestion;

public class AddJobQuestionCommandValidator : AbstractValidator<AddJobQuestionCommand>
{
  public AddJobQuestionCommandValidator()
  {
    RuleFor(x => x.Slug).NotEmpty().MaximumLength(100);
    RuleFor(x => x.Question).NotEmpty().MaximumLength(1000);
  }
}
