using FluentValidation;

namespace Kawadar.Application.Features.Contracts.Queries.GetMyContracts;

public class GetMyContractsQueryValidator : AbstractValidator<GetMyContractsQuery>
{

  public GetMyContractsQueryValidator()
  {
    RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("Page number must be greater than 0.");
    RuleFor(x => x.PageSize).GreaterThan(0).WithMessage("Page size must be greater than 0.");

  }
}