using FluentValidation;
using Kawadar.Application.Common.ExtensionValidator;
using Kawadar.Domain.Portfolios.Items.Enum;

namespace Kawadar.Application.Features.Portfolios.Commands.CreateItem
{
    public class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
    {
        public CreateItemCommandValidator()
        {
            RuleFor(x => x.PortfolioProjectId).NotEmpty().WithMessage("Project id is required")
                .NotEqual(Guid.Empty).WithMessage("Project Id can't be empty");

            RuleFor(x => x.ItemType).IsInEnum();

            When(x => x.ItemType == ItemType.Image, () =>
            {
                RuleFor(x => x.file)
                    .NotNull().WithMessage("File is required when item type is Image");

                RuleFor(x => x.Content).Null().WithMessage("You can't provide content when you create an image item");

                RuleFor(x => x.file!.Length).LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("File size can't exceed 10 MB");

                RuleFor(x => x.file!.FileName).Must(FileName => ExtensionValidator.ValidExtension(FileName, Extensions.AllowedImageExtensions))
                    .WithMessage("The supported file extensions are jpg, jpeg and png");
            });

            When(x => x.ItemType != ItemType.Image, () =>
            {
                RuleFor(x => x.file)
                    .Null().WithMessage("File must be null when item type is not Image");

                RuleFor(x => x.Content).NotNull().WithMessage("Item Content Is required")
                    .NotEmpty().WithMessage("Item Content can't be Empty")
                    .MaximumLength(300).WithMessage("Item Content can't exceed 300 character");
            });
        }
    }
}
