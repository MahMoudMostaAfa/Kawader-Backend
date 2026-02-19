using FluentValidation;
using Kawadar.Application.Common.ExtensionValidator;
using Kawadar.Domain.Portfolios.Items.Enum;

namespace Kawadar.Application.Features.Portfolios.Commands.CreateImageItem
{
    public class CreateImageItemCommandValidator : AbstractValidator<CreateImageItemCommand>
    {
        public CreateImageItemCommandValidator()
        {
            RuleFor(x => x.ItemType).Equal(ItemType.Image).WithMessage("The Item type must be an image");

            RuleFor(x => x.Image).NotNull().WithMessage("Image is required");

            RuleFor(x => x.PortfolioProjectId).NotEmpty().WithMessage("Project id is required")
                .NotEqual(Guid.Empty).WithMessage("Project Id can't be empty");

            RuleFor(x => x.Image.Length).LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("File size can't exceed 10 MB");

            RuleFor(x => x.Image.FileName).Must(FileName => ExtensionValidator.ValidExtension(FileName, Extensions.AllowedImageExtensions))
                .WithMessage("The supported file extensions are jpg, jpeg and png");
        }
    }
}