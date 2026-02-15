using FluentValidation;

namespace Kawadar.Application.Features.Portfolios.Commands.UpdateItem
{
    public class UpdateItemCommandValidator : AbstractValidator<UpdateItemCommand>
    {
        public UpdateItemCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Item Id is required")
                .NotEqual(Guid.Empty).WithMessage("Item Id can't be empty");

            RuleFor(x => x.Content).NotEmpty().WithMessage("Item content can't be empty")
                .MaximumLength(300).WithMessage("Item content can't exceed 300 characters");
        }
    }
}
