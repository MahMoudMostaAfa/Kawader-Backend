using FluentValidation;

namespace Kawadar.Application.Features.Portfolios.Commands.DeleteItem
{
    public class DeleteItemCommandValdiator : AbstractValidator<DeleteItemCommand>
    {
        public DeleteItemCommandValdiator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Item Id is required")
                .NotEqual(Guid.Empty).WithMessage("Item Id can't be empty");
        }
    }
}
