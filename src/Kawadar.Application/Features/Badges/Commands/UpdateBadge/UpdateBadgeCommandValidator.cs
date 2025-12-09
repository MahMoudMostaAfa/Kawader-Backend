

using FluentValidation;
using System.Data;

namespace Kawadar.Application.Features.Badges.Commands.UpdateBadge
{
    public class UpdateBadgeCommandValidator: AbstractValidator<UpdateBadgeCommand>
    {
        public UpdateBadgeCommandValidator()
        {
            RuleFor(x => x.badgeId).NotEmpty().WithMessage("Badge Id is Required")
                .Equal(Guid.Empty).WithMessage("Badge Id can't be empty");

            RuleFor(x => x.IconUrl).NotEmpty().WithMessage("Icon Url can't be empty");
        }
    }
}
