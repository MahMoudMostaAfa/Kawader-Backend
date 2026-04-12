using FluentValidation;

namespace Kawadar.Application.Features.Specilizations.Commands.CreateSpecilization
{
    public class CreateSpecilizationCommandValidator: AbstractValidator<CreateSpecilizationCommand>
    {
        public CreateSpecilizationCommandValidator()
        {
            RuleFor(x => x.name).NotNull().WithMessage("the specilization name is required")
                .NotEmpty().WithMessage("Specilization name can't be empty")
                .MaximumLength(50).WithMessage("Specilization name can't exceed 50 character");
        }
    }
}
