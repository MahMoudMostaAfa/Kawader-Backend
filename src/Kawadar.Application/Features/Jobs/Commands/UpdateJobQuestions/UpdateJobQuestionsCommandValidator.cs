using FluentValidation;

namespace Kawadar.Application.Features.Jobs.Commands.UpdateJobQuestions;

public class UpdateJobQuestionsCommandValidator : AbstractValidator<UpdateJobQuestionsCommand>
{
  public UpdateJobQuestionsCommandValidator()
  {
    RuleFor(x => x.Slug).NotEmpty().MaximumLength(100);

    RuleFor(x => x.Questions).NotNull()
      .Must(q => q.Count <= 5)
      .WithMessage("A job can have a maximum of 5 questions.");

    RuleForEach(x => x.Questions).ChildRules(question =>
    {
      question.RuleFor(q => q.Question).NotEmpty().MaximumLength(1000);
    });
  }
}
