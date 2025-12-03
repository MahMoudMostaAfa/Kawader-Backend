using FluentValidation;

namespace Kawadar.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
  public ResetPasswordCommandValidator()
  {
    RuleFor(c => c.UserId)
      .NotEmpty().WithMessage("User ID is required.");

    RuleFor(c => c.Token)
      .NotEmpty().WithMessage("Token is required.");

    RuleFor(c => c.NewPassword)
      .NotEmpty().WithMessage("New password is required.")
      .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
      .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
      .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
      .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.");
  }
}