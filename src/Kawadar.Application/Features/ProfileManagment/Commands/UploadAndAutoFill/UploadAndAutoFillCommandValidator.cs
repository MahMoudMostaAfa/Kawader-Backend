using FluentValidation;

namespace Kawadar.Application.Features.ProfileManagment.Commands.UploadAndAutoFill;

public class UploadAndAutoFillCommandValidator : AbstractValidator<UploadAndAutoFillCommand>
{
  public UploadAndAutoFillCommandValidator()
  {
    RuleFor(x => x.File)
      .NotNull()
      .WithMessage("File is required.")
      .Must(file => file.ContentType == "application/pdf")
      .WithMessage("Only PDF files are allowed.")
      .Must(file => file.Length <= 5 * 1024 * 1024)
      .WithMessage("File size must be less than 5MB.")
      ;
  }
}