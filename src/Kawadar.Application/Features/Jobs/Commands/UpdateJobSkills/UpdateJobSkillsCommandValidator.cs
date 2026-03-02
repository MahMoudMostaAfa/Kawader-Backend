using FluentValidation;

namespace Kawadar.Application.Features.Jobs.Commands.UpdateJobSkills;

public class UpdateJobSkillsCommandValidator : AbstractValidator<UpdateJobSkillsCommand>
{
  public UpdateJobSkillsCommandValidator()
  {
    RuleFor(x => x.Slug).NotEmpty().MaximumLength(100);

    RuleFor(x => x.SkillIds).NotNull()
      .Must(ids => ids.Count <= 10)
      .WithMessage("A job can have a maximum of 10 skills.");

    RuleForEach(x => x.SkillIds)
      .NotEqual(Guid.Empty)
      .WithMessage("Skill ID must not be empty.");
  }
}
