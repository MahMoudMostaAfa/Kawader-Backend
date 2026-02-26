using FluentValidation;

namespace Kawadar.Application.Features.Skills.Commands.AddSkillsToProject
{
    public class AddSkillsToProjectValidator : AbstractValidator<AddSkillsToProjectCommand>
    {
        public AddSkillsToProjectValidator()
        {
            RuleFor(x => x.skills).NotNull().WithMessage("Skills List can't be null")
                .NotEmpty().WithMessage("Skills List must have at least one item");

            RuleForEach(x => x.skills).ChildRules(item =>
            {
                item.RuleFor(x => x.SkillId).NotEqual(Guid.Empty).WithMessage("Skill Id can't be empty");
            });
        }
    }
}
