using FluentValidation;
using Kawadar.Domain.Contracts.Disbutes.Enum;

namespace Kawadar.Application.Features.Contracts.Disbutes.Commands.SolveDisbute
{
    public class SolveDisbuteCommandValidator : AbstractValidator<SolveDisbuteCommand>
    {
        public SolveDisbuteCommandValidator()
        {
            RuleFor(x => x.status).IsInEnum();

            RuleFor(x => x.resolution).NotNull().When(x => x.status == DisbuteStatus.Resolved)
                .WithMessage("The resolution is required")
                .NotEmpty().When(x => x.status == DisbuteStatus.Resolved).WithMessage("The resolution can't be empty");

            RuleFor(x => x.DisbuteId).NotNull().WithMessage("The disbute Id is required")
                .NotEqual(Guid.Empty).WithMessage("The disbute Id can't be empty");
        }
    }
}
