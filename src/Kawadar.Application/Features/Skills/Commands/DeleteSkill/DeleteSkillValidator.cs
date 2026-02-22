using FluentValidation;

namespace Kawadar.Application.Features.Skills.Commands.DeleteSkill
{
    public class DeleteSkillValidator : AbstractValidator<DeleteSkillCommand>
    {
        public DeleteSkillValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("Skill Id is required")
                .NotEmpty().WithMessage("Skill Id can't be empty");
        }
    }
}
