using FluentValidation;

namespace Kawadar.Application.Features.Violations.Commands.SolveViolation
{
    public class SolveViolationCommandValidator : AbstractValidator<SolveViolationCommand>
    {
        public SolveViolationCommandValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("The Violation Id is Required")
                .NotEqual(Guid.Empty).WithMessage("The Violation can't be Empty");

            RuleFor(x => x.noteByAdmin).NotNull().WithMessage("The admin note is required")
                .NotEmpty().WithMessage("The admin note can't be empty");

            RuleFor(x => x.action).NotNull().When(x => x.status == Domain.Violations.Enums.ViolationStatus.Resolved).WithMessage("The Taken action is required")
                .NotEmpty().When(x => x.status == Domain.Violations.Enums.ViolationStatus.Resolved).WithMessage("The Taken action can't be empty");

            RuleFor(x => x.status).IsInEnum();
        }
    }
}
