using FluentValidation;

namespace Kawadar.Application.Features.Skills.Queries.GetSkillById
{
    public class GetSkillByIdValidator : AbstractValidator<GetSkillByIdQuery>
    {
        public GetSkillByIdValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("Skill Id is required")
                .NotEqual(Guid.Empty).WithMessage("Skill Id can't be empty");
        }
    }
}
