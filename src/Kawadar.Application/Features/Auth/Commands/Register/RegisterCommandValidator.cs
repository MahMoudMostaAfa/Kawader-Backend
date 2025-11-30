using FluentValidation;

namespace Kawadar.Application.Features.Auth.Commands.Register;


public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
  public RegisterCommandValidator()
  {
    RuleFor(x => x.name)
      .NotEmpty().WithMessage("Name is required.")
      .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
      .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
  }
}