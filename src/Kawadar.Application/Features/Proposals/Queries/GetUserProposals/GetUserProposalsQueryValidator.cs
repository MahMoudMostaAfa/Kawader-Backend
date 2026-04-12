using FluentValidation;

namespace Kawadar.Application.Features.Proposals.Queries.GetUserProposals;

public class GetUserProposalsQueryValidator : AbstractValidator<GetUserProposalsQuery>
{

  public GetUserProposalsQueryValidator()
  {
    RuleFor(x => x.SortBy).Must(x => x == "newest" || x == "oldest").WithMessage("DatesortBy must be either 'newest' or 'oldest'");
    RuleFor(x => x.PageSize).GreaterThan(0).WithMessage("page size must big than 0");
    RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("page number must big than 0");

  }
}