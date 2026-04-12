using FluentValidation;

namespace Kawadar.Application.Features.Skills.Commands.RemoveSkillFromFreelancer
{
    public class RemoveSkillFromFreelancerCommandValidator: AbstractValidator<RemoveSkillFromFreelancerCommand>
    {
        public RemoveSkillFromFreelancerCommandValidator()
        {
            RuleFor(x => x.skillName).NotEmpty().WithMessage("skill name can't be empty")
                .NotNull().WithMessage("Skill name can't be null")
                .MaximumLength(50).WithMessage("Skill name can't be greater than 50 character");
        }
    }
}
