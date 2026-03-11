using FluentValidation;

namespace Kawadar.Application.Features.Jobs.Commands.UpdateJobQuestion;

public class UpdateJobQuestionCommandValidator : AbstractValidator<UpdateJobQuestionCommand>
{
  public UpdateJobQuestionCommandValidator()
  {
    RuleFor(x => x.Slug).NotEmpty().MaximumLength(100);
    RuleFor(x => x.QuestionId).NotEmpty();
    RuleFor(x => x.Question).NotEmpty().MaximumLength(1000);
  }
}
