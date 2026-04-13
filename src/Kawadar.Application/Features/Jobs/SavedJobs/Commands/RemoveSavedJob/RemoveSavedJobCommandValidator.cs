using FluentValidation;

namespace Kawadar.Application.Features.Jobs.SavedJobs.Commands.RemoveSavedJob;


public class RemoveSavedJobCommandValidator : AbstractValidator<RemoveSavedJobCommand>
{
  public RemoveSavedJobCommandValidator()
  {
    RuleFor(x => x.JobId).NotEmpty().WithMessage("JobId is required");
  }
}