using FluentValidation;

namespace Kawadar.Application.Features.Auth.Commands.ResendConfirmationEmail;

public class ResendConfirmationEmailCommandValidator : AbstractValidator<ResendConfirmationEmailCommand>
{
  public ResendConfirmationEmailCommandValidator()
  {
    RuleFor(c => c.Email)
      .NotEmpty().WithMessage("Email is required.")
      .EmailAddress().WithMessage("A valid email address is required.");
  }
}