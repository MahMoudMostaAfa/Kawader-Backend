using FluentValidation;
using Kawadar.Application.Common.ExtensionValidator;
using Kawadar.Domain.Portfolios.Items.Enum;

namespace Kawadar.Application.Features.Portfolios.Commands.UpdateItem
{
    public class UpdateItemCommandValidator : AbstractValidator<UpdateItemCommand>
    {
        public UpdateItemCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Item Id is required")
                .NotEqual(Guid.Empty).WithMessage("Item Id can't be empty");

            RuleFor(x => x.itemType).IsInEnum();

            When(x => x.itemType != ItemType.Image, () =>
            {
                RuleFor(x => x.Image).Null().WithMessage("When the item type is text or link you can't add a file");

                RuleFor(x => x.Content).NotNull().WithMessage("Item content is required")
                    .NotEmpty().WithMessage("Item content can't be empty")
                    .MaximumLength(300).WithMessage("Item content can't exceed 300 characters");
            });

            When(x => x.itemType == ItemType.Image, () =>
            {
                RuleFor(x => x.Image)
                    .NotNull().WithMessage("File is required when item type is Image");

                RuleFor(x => x.Content).Null().WithMessage("You can't provide content when you create an image item");

                RuleFor(x => x.Image!.Length).LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("File size can't exceed 10 MB");

                RuleFor(x => x.Image!.FileName).Must(FileName => ExtensionValidator.ValidExtension(FileName, Extensions.AllowedImageExtensions))
                    .WithMessage("The supported file extensions are jpg, jpeg and png");
            });
        }
    }
}
