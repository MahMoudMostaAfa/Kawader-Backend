

using FluentValidation;
using Kawadar.Application.Common.ExtensionValidator;
using System.Data;

namespace Kawadar.Application.Features.Badges.Commands.UpdateBadge
{
    public class UpdateBadgeCommandValidator: AbstractValidator<UpdateBadgeCommand>
    {
        public UpdateBadgeCommandValidator()
        {
            RuleFor(x => x.badgeId).NotEmpty().WithMessage("Badge Id is Required")
                .NotEqual(Guid.Empty).WithMessage("Badge Id can't be empty");

            RuleFor(x => x.Icon).NotNull().WithMessage("The new icon is required to update");

            RuleFor(x => x.Icon.Length).LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("File size can't exceed 10 MB");

            RuleFor(x => x.Icon.FileName).Must(FileName => ExtensionValidator.ValidExtension(FileName, Extensions.AllowedImageExtensions))
                .WithMessage("The supported file extensions are jpg, jpeg and png");
        }
    }
}
