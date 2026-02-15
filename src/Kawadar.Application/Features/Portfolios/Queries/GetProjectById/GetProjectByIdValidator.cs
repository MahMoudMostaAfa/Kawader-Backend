using FluentValidation;

namespace Kawadar.Application.Features.Portfolios.Queries.GetProjectById
{
    public class GetProjectByIdValidator : AbstractValidator<GetProjectByIdQuery>
    {
        public GetProjectByIdValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Project Id is required")
                .NotEqual(Guid.Empty).WithMessage("Project Id can't be empty");
        }
    }
}
