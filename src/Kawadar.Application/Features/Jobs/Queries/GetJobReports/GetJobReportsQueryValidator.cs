using FluentValidation;

namespace Kawadar.Application.Features.Jobs.Queries.GetJobReports
{
    public class GetJobReportsQueryValidator : AbstractValidator<GetJobReportsQuery>
    {
        public GetJobReportsQueryValidator()
        {
            RuleFor(x => x.reportStatus).IsInEnum().When(x => x.reportStatus is not null);
            RuleFor(x => x.reportType).IsInEnum().When(x => x.reportType is not null);
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);

            RuleFor(x => x.SortBy)
              .Must(s => s is "newest" or "oldest")
              .WithMessage("SortBy must be 'newest' or 'oldest'.");
        }
    }
}
