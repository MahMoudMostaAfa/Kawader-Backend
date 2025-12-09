using FluentValidation;

namespace Kawadar.Application.Features.Specilizations.Queries.GetSpecilizationById
{
    public class GetSpecilizationByIdQueryValidator: AbstractValidator<GetSpecilizationByIdQuery>
    {
        public GetSpecilizationByIdQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Specilization Id is required")
                .NotEqual(Guid.Empty).WithMessage("Specilization Id can't be empty");
        }
    }
}
