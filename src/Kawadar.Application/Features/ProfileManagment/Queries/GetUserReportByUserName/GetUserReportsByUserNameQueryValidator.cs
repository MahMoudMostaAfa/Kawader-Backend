using FluentValidation;

namespace Kawadar.Application.Features.ProfileManagment.Queries.GetUserReportByUserName
{
    public class GetUserReportsByUserNameQueryValidator : AbstractValidator<GetUserReportByUserNameQuery>
    {
        public GetUserReportsByUserNameQueryValidator()
        {
            RuleFor(x => x.userName).NotNull().WithMessage("UserName is required").
                NotEmpty().WithMessage("UserName can't be empty")
                .MaximumLength(50).WithMessage("UserName can't exceed 50 character");
            RuleFor(x => x.reportStatus).IsInEnum().When(x => x.reportStatus is not null);
            RuleFor(x => x.reportType).IsInEnum().When(x => x.reportType is not null);
            RuleFor(x => x.page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.pageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);

            RuleFor(x => x.sortBy)
              .Must(s => s is "newest" or "oldest")
              .WithMessage("SortBy must be 'newest' or 'oldest'.");
        }
    }
}
