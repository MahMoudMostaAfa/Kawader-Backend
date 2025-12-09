using FluentValidation;

namespace Kawadar.Application.Features.Specilizations.Commands.CreateSpecilization
{
    public class CreateSpecilizationCommandValidator: AbstractValidator<CreateSpecilizationCommand>
    {
        public CreateSpecilizationCommandValidator()
        {
            RuleFor(x => x.name).NotEmpty().WithMessage("Specilization name can't be empty");
        }
    }
}
