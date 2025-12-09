
using FluentValidation;

namespace Kawadar.Application.Features.Badges.Commands.CreateBadge
{
    public class CreateBadgeCommandValidator: AbstractValidator<CreateBadgeCommand>
    {
        public CreateBadgeCommandValidator()
        {
            RuleFor(x => x.title).NotEmpty().WithMessage("Title Can't be Empty");

            RuleFor(x => x.description).NotEmpty().WithMessage("Description Can't be Empty");

            RuleFor(x => x.IconUrl).NotEmpty().WithMessage("Icon Can't be Empty");
        }
    }
}
