using FluentValidation;

namespace Kawadar.Application.Features.Contracts.Queries.GetContractDetails;

public class GetContractDetailsQueryValidator : AbstractValidator<GetContractDetailsQuery>
{
  public GetContractDetailsQueryValidator()
  {
    RuleFor(x => x.ContractId).NotEmpty().WithMessage("ContractId is required");
  }
}