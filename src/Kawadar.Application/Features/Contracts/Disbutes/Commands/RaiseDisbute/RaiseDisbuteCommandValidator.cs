using FluentValidation;

namespace Kawadar.Application.Features.Contracts.Disbutes.Commands.RaiseDisbute
{
    public class RaiseDisbuteCommandValidator : AbstractValidator<RaiseDisbuteCommand>
    {
        public RaiseDisbuteCommandValidator()
        {
            RuleFor(x => x.reason).NotNull().WithMessage("The disbute reason is required")
                .NotEmpty().WithMessage("The disbute reason can't be empty")
                .MaximumLength(500).WithMessage("The disbute reason can't exceed 500 characters");

            RuleFor(x => x.ContractId).NotNull().WithMessage("The Contract Id is required")
                .NotEqual(Guid.Empty).WithMessage("The Contract Id can't be empty");
        }
    }
}
