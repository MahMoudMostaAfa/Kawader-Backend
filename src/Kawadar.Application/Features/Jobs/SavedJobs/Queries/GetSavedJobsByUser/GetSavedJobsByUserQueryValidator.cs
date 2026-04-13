using FluentValidation;
using MediatR;

namespace Kawadar.Application.Features.Jobs.SavedJobs.Queries.GetSavedJobsByUser;


public class GetSavedJobsByUserQueryValidator : AbstractValidator<GetSavedJobsByUserQuery>
{
  public GetSavedJobsByUserQueryValidator()
  {
    RuleFor(x => x.PageNumber)
      .GreaterThan(0).WithMessage("Page number must be greater than 0.");
    RuleFor(x => x.PageSize)
      .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
  }
}
