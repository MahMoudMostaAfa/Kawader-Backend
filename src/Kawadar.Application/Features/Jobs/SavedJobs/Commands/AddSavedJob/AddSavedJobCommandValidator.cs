using FluentValidation;

namespace Kawadar.Application.Features.Jobs.SavedJobs.Commands.AddSavedJob;


public class AddSavedJobCommandValidator : AbstractValidator<AddSavedJobCommand>
{
  public AddSavedJobCommandValidator()
  {
    RuleFor(x => x.JobId).NotEmpty().WithMessage("JobId is required.");
  }
}