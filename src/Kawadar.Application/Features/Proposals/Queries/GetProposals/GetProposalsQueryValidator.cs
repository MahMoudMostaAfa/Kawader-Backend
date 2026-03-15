using FluentValidation;

namespace Kawadar.Application.Features.Proposals.Queries.GetProposals;

public class GetProposalsQueryValidator : AbstractValidator<GetProposalsQuery>
{

  public GetProposalsQueryValidator()
  {
    RuleFor(x => x.JobId).NotEmpty().WithMessage("JobId is required");

    RuleFor(x => x.Page).GreaterThan(0).WithMessage("Page must be greater than 0");
    RuleFor(x => x.PageSize).GreaterThan(0).WithMessage("PageSize must be greater than 0");
    RuleFor(x => x.DatesortBy).Must(x => x == "newest" || x == "oldest").WithMessage("DatesortBy must be either 'newest' or 'oldest'");
    RuleFor(x => x.PriceSortBy).Must(x => x == null || x == "lowest" || x == "highest").WithMessage("PriceSortBy must be either 'lowest', 'highest' or null");
    RuleFor(x => x.EstimatedTimeSortBy).Must(x => x == null || x == "shortest" || x == "longest").WithMessage("EstimatedTimeSortBy must be either 'shortest', 'longest' or null");

  }
}