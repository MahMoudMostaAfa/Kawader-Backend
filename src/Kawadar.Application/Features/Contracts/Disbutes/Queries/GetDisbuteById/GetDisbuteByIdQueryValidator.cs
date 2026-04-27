using FluentValidation;

namespace Kawadar.Application.Features.Contracts.Disbutes.Queries.GetDisbuteById
{
    public class GetDisbuteByIdQueryValidator : AbstractValidator<GetDisbuteByIdQuery>
    {
        public GetDisbuteByIdQueryValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("The disbute Id is required")
                .NotEqual(Guid.Empty).WithMessage("The disbute Id can't be empty");
        }
    }
}
