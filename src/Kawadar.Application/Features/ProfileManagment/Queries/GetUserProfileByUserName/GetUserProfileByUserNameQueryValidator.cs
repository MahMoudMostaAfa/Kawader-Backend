using FluentValidation;

namespace Kawadar.Application.Features.ProfileManagment.Queries.GetUserProfileByUserName;



public class GetUserProfileByUserNameQueryValidator : AbstractValidator<GetUserProfileByUserNameQuery>
{
  public GetUserProfileByUserNameQueryValidator()
  {
    RuleFor(x => x.UserName)
    .NotEmpty().WithMessage("Username is required.")
    .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
    .MaximumLength(100).WithMessage("Username must not exceed 100 characters.");
  }
}