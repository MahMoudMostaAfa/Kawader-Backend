
using FluentValidation;
using Kawadar.Application.Common.ExtensionValidator;

namespace Kawadar.Application.Features.Portfolios.Commands.UpdateImageItem
{
    public class UpdateImageItemValidator : AbstractValidator<UpdateImageItemCommand>
    {
        public UpdateImageItemValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Item Id is required")
                .NotEqual(Guid.Empty).WithMessage("Item Id can't be empty");

            RuleFor(x => x.Image).NotNull().WithMessage("Provide an Image to update Item");

            RuleFor(x => x.Image.FileName).Must(FileName => ExtensionValidator.ValidExtension(FileName, Extensions.AllowedImageExtensions))
                .WithMessage("Allowed extensions are png, jpg and jepg");

            RuleFor(x => x.Image.Length).LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("Image can't exceed 10 MB");
        }
    }
}
