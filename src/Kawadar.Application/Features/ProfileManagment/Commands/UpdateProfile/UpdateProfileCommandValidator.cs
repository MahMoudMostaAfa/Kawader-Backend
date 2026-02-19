using FluentValidation;

namespace Kawadar.Application.Features.ProfileManagment.Commands.UpdateProfile;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
  public UpdateProfileCommandValidator()
  {
    RuleFor(x => x.FirstName).MaximumLength(50).WithMessage("First name must not exceed 50 characters.");
    RuleFor(x => x.LastName).MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");
    RuleFor(x => x.Title).MaximumLength(100).WithMessage("Title must not exceed 100 characters.");
    RuleFor(x => x.Bio).MaximumLength(1000).WithMessage("Bio must not exceed 1000 characters.");
    RuleFor(x => x.PhoneNumber).Matches(@"^01[0125]\d{8}$").WithMessage("Phone number must be in a valid format.");
    RuleFor(x => x.ExperienceYear).IsInEnum().When(x => x.ExperienceYear.HasValue).WithMessage("Experience year must be a valid enum value.");
    RuleFor(x => x.ProfileType).IsInEnum().When(x => x.ProfileType.HasValue).WithMessage("Profile type must be a valid enum value.");
    // isavailable is a nullable boolean, so no need for validation as it can be true, false, or null (not provided).
    RuleFor(x => x.IsAvailable).Must(x => x == true || x == false || x == null).WithMessage("IsAvailable must be true, false, or null.");





  }

}