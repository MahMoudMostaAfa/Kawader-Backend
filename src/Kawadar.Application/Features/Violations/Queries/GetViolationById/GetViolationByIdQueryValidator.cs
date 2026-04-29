using FluentValidation;

namespace Kawadar.Application.Features.Violations.Queries.GetViolationById
{
    public class GetViolationByIdQueryValidator : AbstractValidator<GetViolationByIdQuery>
    {
        public GetViolationByIdQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Violation Id is required")
                .NotEqual(Guid.Empty).WithMessage("Violation9 Id can't be empty");
        }
    }
}
