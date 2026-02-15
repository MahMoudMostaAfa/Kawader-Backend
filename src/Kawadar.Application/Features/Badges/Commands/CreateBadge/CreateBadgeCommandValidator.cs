
using FluentValidation;
using Kawadar.Application.Common.ExtensionValidator;

namespace Kawadar.Application.Features.Badges.Commands.CreateBadge
{
    public class CreateBadgeCommandValidator: AbstractValidator<CreateBadgeCommand>
    {
        public CreateBadgeCommandValidator()
        {
            RuleFor(x => x.title).NotEmpty().WithMessage("Title Can't be Empty");

            RuleFor(x => x.description).NotEmpty().WithMessage("Description Can't be Empty");

            RuleFor(x => x.Icon).NotNull().WithMessage("Icon is required");

            RuleFor(x => x.Icon.Length).LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("File size can't exceed 10 MB");

            RuleFor(x => x.Icon.FileName).Must(FileName => ExtensionValidator.ValidExtension(FileName, Extensions.AllowedImageExtensions))
                .WithMessage("The supported file extensions are jpg, jpeg and png");
        }
    }
}
