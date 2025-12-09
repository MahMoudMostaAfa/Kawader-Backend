
using FluentValidation;

namespace Kawadar.Application.Features.Badges.Commands.DeleteBadge
{
    public class DeleteBadgeCommandValidator: AbstractValidator<DeleteBadgeCommand>
    {
        public DeleteBadgeCommandValidator()
        {
            RuleFor(x => x.badgeId).NotEmpty().WithMessage("Badge Id is required")
                .NotEqual(Guid.Empty).WithMessage("Badge Id can't be empty");
        }
    }
}
