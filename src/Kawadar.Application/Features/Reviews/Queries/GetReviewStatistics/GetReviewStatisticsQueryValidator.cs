using FluentValidation;

namespace Kawadar.Application.Features.Reviews.Queries.GetReviewStatistics
{
    public class GetReviewStatisticsQueryValidator : AbstractValidator<GetReviewStatisticsQuery>
    {
        public GetReviewStatisticsQueryValidator()
        {
            RuleFor(x => x.userName).NotNull().WithMessage("UserName is required")
                .NotEmpty().WithMessage("UserName can't be empty");
        }
    }
}
