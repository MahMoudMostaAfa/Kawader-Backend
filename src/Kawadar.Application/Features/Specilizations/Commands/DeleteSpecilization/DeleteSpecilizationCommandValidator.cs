using FluentValidation;

namespace Kawadar.Application.Features.Specilizations.Commands.DeleteSpecilization
{
    public class DeleteSpecilizationCommandValidator : AbstractValidator<DeleteSpecilizationCommand>
    {
        public DeleteSpecilizationCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Specilization Id is required")
                .NotEqual(Guid.Empty).WithMessage("Specilization Id can't be empty");
        }
    }
}
