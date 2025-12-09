using FluentValidation;

namespace Kawadar.Application.Features.Portfolios.Queries.GetProjectItemsById
{
    public class GetProjectItemsByIdValidator : AbstractValidator<GetProjectItemsByIdQuery>
    {
        public GetProjectItemsByIdValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Project Id is Required")
                .NotEqual(Guid.Empty).WithMessage("Project Id can't be Empty");
        }
    }
}
