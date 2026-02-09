
using FluentValidation;

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

            RuleFor(x => x.Icon.Name).Must(HaveValidExtension).WithMessage("The supported file extensions are jpg, jpeg and png");
        }

        private bool HaveValidExtension(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }
    }
}
