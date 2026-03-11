using FluentValidation;

namespace Kawadar.Application.Features.Reviews.Queries.GetReviewsByUserProfileId
{
    public class GetReviewsByUserNameQueryValidator : AbstractValidator<GetReviewsByUserNameQuery>
    {
        private string[] _sort = { "newest", "oldest", "highest", "lowest"};
        public GetReviewsByUserNameQueryValidator()
        {
            RuleFor(x => x.page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.pageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);

            RuleFor(x => x.sortBy)
              .Must(_sort.Contains)
              .WithMessage("SortBy must be one of theses values: 'newest', 'oldest', 'lowest', 'highest'.");

            RuleFor(x => x.rating).InclusiveBetween(0, 5).WithMessage("Rating must be between 0 and 5");

            RuleFor(x => x.userName).NotNull().WithMessage("UserName is required")
                .NotEmpty().WithMessage("UserName can't be empty");
        }
    }
}
