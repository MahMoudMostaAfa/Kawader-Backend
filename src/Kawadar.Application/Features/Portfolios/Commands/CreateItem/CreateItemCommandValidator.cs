using FluentValidation;
using Kawadar.Domain.Portfolios.Items.Enum;

namespace Kawadar.Application.Features.Portfolios.Commands.CreateItem
{
    public class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
    {
        public CreateItemCommandValidator()
        {
            RuleFor(x => x.ItemType).NotEqual(ItemType.Image).WithMessage("The Type must be a Link or Text");

            RuleFor(x => x.Content).NotEmpty().WithMessage("Item Content is required")
                .MaximumLength(300).WithMessage("Item Content can't exceed 300 character");

            RuleFor(x => x.PortfolioProjectId).NotEmpty().WithMessage("Project id is required")
                .NotEqual(Guid.Empty).WithMessage("Project Id can't be empty");
        }
    }
}
