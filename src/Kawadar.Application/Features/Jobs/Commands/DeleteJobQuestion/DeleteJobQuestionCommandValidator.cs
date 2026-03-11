using FluentValidation;

namespace Kawadar.Application.Features.Jobs.Commands.DeleteJobQuestion;

public class DeleteJobQuestionCommandValidator : AbstractValidator<DeleteJobQuestionCommand>
{
  public DeleteJobQuestionCommandValidator()
  {
    RuleFor(x => x.Slug).NotEmpty().MaximumLength(100);
    RuleFor(x => x.QuestionId).NotEmpty();
  }
}
