using FluentValidation;

namespace Kawadar.Application.Features.ProfileManagment.Commands.UploadIdentity;


public class UploadIdentityCommandValidator : AbstractValidator<UploadIdentityCommand>
{

  public UploadIdentityCommandValidator()
  {
    RuleFor(x => x.FrontImage)
        .NotNull().WithMessage("Front image is required.")
        .Must(file => file.ContentType.StartsWith("image/")).WithMessage("Front image must be a valid image file.")
        .Must(file => file.Length <= 5 * 1024 * 1024).WithMessage("Front image must be less than 5MB.")

        ;
    RuleFor(x => x.BackImage)
        .NotNull().WithMessage("Back image is required.")
        .Must(file => file.ContentType.StartsWith("image/")).WithMessage("Back image must be a valid image file.")
        .Must(file => file.Length <= 5 * 1024 * 1024).WithMessage("Back image must be less than 5MB.");
  }
}