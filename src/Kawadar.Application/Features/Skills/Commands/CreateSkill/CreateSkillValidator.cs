using FluentValidation;

namespace Kawadar.Application.Features.Skills.Commands.CreateSkill
{
    public class CreateSkillValidator : AbstractValidator<CreateSkillCommand>
    {
        public CreateSkillValidator()
        {
            RuleFor(x => x.name).NotNull().WithMessage("Skill name is required")
                .NotEmpty().WithMessage("Skill name can't be empty");
        }
    }
}
