using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.CreatePaymentIntention;

public class CreatePaymentIntentionCommandValidator : AbstractValidator<CreatePaymentIntentionCommand>
{
  public CreatePaymentIntentionCommandValidator()
  {
    RuleFor(x => x.Amount)
      .GreaterThan(0)
      .WithMessage("Amount must be greater than zero.")
      .LessThanOrEqualTo(100000)
      .WithMessage("Amount cannot exceed 100,000 EGP.");
  }
}
