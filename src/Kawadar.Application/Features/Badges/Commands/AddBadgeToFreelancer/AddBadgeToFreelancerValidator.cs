using FluentValidation;
using System.Drawing;

namespace Kawadar.Application.Features.Badges.Commands.AddBadgeToFreelancer
{
    public class AddBadgeToFreelancerValidator: AbstractValidator<AddBadgeToFreelancerCommand>
    {
        public AddBadgeToFreelancerValidator()
        {
            RuleFor(x => x.BadgeId).NotEmpty().WithMessage("Badge Id is required")
                .NotEqual(Guid.Empty).WithMessage("Badge Id can't be empty");

            RuleFor(x => x.FreelancerId).NotEmpty().WithMessage("Freelancer Id is required")
                .NotEqual(Guid.Empty).WithMessage("Freelancer Id can't be empty");
        }
    }
}
