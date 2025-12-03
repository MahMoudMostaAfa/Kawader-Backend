using FluentValidation;

namespace Kawadar.Application.Features.Auth.Commands.ForgetPassword;

public class ForgetPasswordCommandValidator : AbstractValidator<ForgetPasswordCommand>
{
  public ForgetPasswordCommandValidator()
  {
    RuleFor(c => c.Email)
      .NotEmpty().WithMessage("Email is required.")
      .EmailAddress().WithMessage("A valid email address is required.");
  }
}