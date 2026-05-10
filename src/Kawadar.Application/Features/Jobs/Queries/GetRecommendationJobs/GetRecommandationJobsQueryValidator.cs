using FluentValidation;

namespace Kawadar.Application.Features.Jobs.Queries.GetRecommendationJobs;


public class GetRecommandationJobsQueryValidator : AbstractValidator<GetRecommandationJobsQuery>
{
  public GetRecommandationJobsQueryValidator()
  {
    RuleFor(x => x.page).GreaterThan(0).WithMessage("Page number must be greater than 0.");
    RuleFor(x => x.pageSize).GreaterThan(0).WithMessage("Page size must be greater than 0.");
  }
}