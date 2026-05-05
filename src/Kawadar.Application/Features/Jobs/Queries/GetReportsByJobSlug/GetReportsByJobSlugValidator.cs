using FluentValidation;

namespace Kawadar.Application.Features.Jobs.Queries.GetReportsByJobSlug
{
    public class GetReportsByJobSlugValidator : AbstractValidator<GetReportsByJobSlugQuery>
    {
        public GetReportsByJobSlugValidator()
        {
            RuleFor(x => x.JobSlug)
                .NotEmpty().WithMessage("Slug is required.")
                .MaximumLength(100).WithMessage("Slug must not exceed 100 characters.");

            RuleFor(x => x.status).IsInEnum().When(x => x.status is not null);

            RuleFor(x => x.type).IsInEnum().When(x => x.type is not null);
            RuleFor(x => x.page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.pageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);

            RuleFor(x => x.sortBy)
              .Must(s => s is "newest" or "oldest")
              .WithMessage("SortBy must be 'newest' or 'oldest'.");
        }
    }
}
