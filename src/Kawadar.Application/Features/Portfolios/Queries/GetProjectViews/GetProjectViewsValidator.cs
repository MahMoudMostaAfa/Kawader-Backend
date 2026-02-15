using FluentValidation;

namespace Kawadar.Application.Features.Portfolios.Queries.GetProjectViews
{
    public class GetProjectViewsValidator : AbstractValidator<GetProjectViewsQuery>
    {
        public GetProjectViewsValidator()
        {
            RuleFor(x => x.projectId).NotEmpty().WithMessage("Project Id is Required")
                .NotEqual(Guid.Empty).WithMessage("Project Id can't be Empty");
        }
    }
}
