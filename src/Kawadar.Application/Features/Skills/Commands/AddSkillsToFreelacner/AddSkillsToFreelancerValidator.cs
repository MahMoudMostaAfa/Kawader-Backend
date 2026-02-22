using FluentValidation;
using Kawadar.Domain.Skills.FreelancerSkill.Enum;

namespace Kawadar.Application.Features.Skills.Commands.AddSkillsToFreelacner
{
    public class AddSkillsToFreelancerValidator : AbstractValidator<AddSkillsToFreelacnerCommand>
    {
        public AddSkillsToFreelancerValidator()
        {
            RuleFor(x => x.Skills).NotNull().WithMessage("Skills List can't be null")
                .NotEmpty().WithMessage("Skills List must have at least one item");

            RuleForEach(x => x.Skills).ChildRules(Item =>
            {
                Item.When(x => x.SkillType == SkillType.Custom, () =>
                {
                    Item.RuleFor(x => x.SkillId).Null().WithMessage("Custom made skills can't point to existing skills");

                    Item.RuleFor(x => x.CustomSkillName).NotNull().WithMessage("CustomSkillName is required for custom skills")
                    .NotEmpty().WithMessage("Custom skill name can't be empty");
                });

                Item.When(x => x.SkillType == SkillType.Predefined, () =>
                {
                    Item.RuleFor(x => x.SkillId).NotNull().WithMessage("Skill Id is required for predefined skills")
                    .NotEqual(Guid.Empty).WithMessage("Skill Id can't be empty for predefined skills");

                    Item.RuleFor(x => x.CustomSkillName).Null().WithMessage("Custom skill name not needed for predefined skills");
                });
            });
        }
    }
}
