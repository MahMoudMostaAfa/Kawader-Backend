using FluentValidation;

namespace Kawadar.Application.Features.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
  public LoginCommandValidator()
  {

    RuleFor(u => u.Email)
    .NotEmpty().WithMessage("Email is required.")
    .EmailAddress().WithMessage("A valid email is required.");


    RuleFor(u => u.Password)
        .NotEmpty().WithMessage("Password is required.")
        .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
      .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
      .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
      .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.");
  }
}