using FluentValidation;

namespace Kawadar.Application.Features.Jobs.Queries.GetJobBySlug;

public class GetJobBySlugQueryValidator : AbstractValidator<GetJobBySlugQuery>
{
  public GetJobBySlugQueryValidator()
  {
    RuleFor(x => x.Slug)
    .NotEmpty().WithMessage("Slug is required.")
    .MaximumLength(100).WithMessage("Slug must not exceed 100 characters.");

  }
}