using FluentValidation;

namespace Kawadar.Application.Features.Jobs.Queries.GetJobById;

public class GetJobByIdQueryValidator : AbstractValidator<GetJobByIdQuery>
{
  public GetJobByIdQueryValidator()
  {
    RuleFor(x => x.JobId)
        .NotEmpty().WithMessage("Job ID is required.");
  }
}