using FluentValidation;

namespace Kawadar.Application.Features.ProfileManagment.Queries.GetFreelancersByAi;

public class GetFreelancersByAiQueryValidator : AbstractValidator<GetFreelancersByAiQuery>
{
  public GetFreelancersByAiQueryValidator()
  {
    RuleFor(x => x.Query)
        .NotEmpty().WithMessage("Query cannot be empty.")
        .MaximumLength(700).WithMessage("Query cannot exceed 700 characters.");
  }
}