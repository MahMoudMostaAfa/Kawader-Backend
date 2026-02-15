
using FluentValidation;

namespace Kawadar.Application.Features.Specilizations.Commands.UpdateSpecilization
{
    public class UpdateSpecilizationCommandValidator : AbstractValidator<UpdateSpecilizationCommand>
    {
        public UpdateSpecilizationCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Specilization Id is required")
                .NotEqual(Guid.Empty).WithMessage("Specilization Id can't be empty");

            RuleFor(x => x.name).NotEmpty().WithMessage("Specilization name can't be empty");
        }
    }
}
