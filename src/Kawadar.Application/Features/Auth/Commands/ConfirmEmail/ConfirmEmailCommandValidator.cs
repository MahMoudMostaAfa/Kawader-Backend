using FluentValidation;

namespace Kawadar.Application.Features.Auth.Commands.ConfirmEmail;

public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
  public ConfirmEmailCommandValidator()
  {
    RuleFor(c => c.UserId)
      .NotEmpty().WithMessage("User ID is required.");

    RuleFor(c => c.Token)
      .NotEmpty().WithMessage("Confirmation token is required.");
  }
}