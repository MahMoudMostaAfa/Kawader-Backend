using FluentValidation;

namespace Kawadar.Application.Features.ProfileManagment.Commands.UpdateProfileImage;

public class UpdateProfileImageCommandValidator : AbstractValidator<UpdateProfileImageCommand>
{
  public UpdateProfileImageCommandValidator()
  {
    RuleFor(x => x.ProfilePic)
        .NotNull().WithMessage("Profile picture is required.")
        .Must(file => file.ContentType.StartsWith("image/")).WithMessage("Only image files are allowed.")
        .Must(file => file.Length <= 5 * 1024 * 1024).WithMessage("Profile picture must be less than 5MB.");
  }
}