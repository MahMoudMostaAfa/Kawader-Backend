using FluentValidation;

namespace Kawadar.Application.Features.Jobs.Queries.GetJobs;

public class GetJobsQueryValidator : AbstractValidator<GetJobsQuery>
{
  public GetJobsQueryValidator()
  {
    RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
    RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);

    RuleFor(x => x.SortBy)
      .Must(s => s is "newest" or "oldest")
      .WithMessage("SortBy must be 'newest' or 'oldest'.");

    RuleFor(x => x.Search).MaximumLength(200).When(x => x.Search is not null);
    RuleFor(x => x.JobType).IsInEnum().When(x => x.JobType is not null);
    RuleFor(x => x.ExperienceLevel).IsInEnum().When(x => x.ExperienceLevel is not null);
    RuleFor(x => x.BudgetRange).IsInEnum().When(x => x.BudgetRange is not null);
    RuleFor(x => x.HourlyRateRange).IsInEnum().When(x => x.HourlyRateRange is not null);
  }
}
