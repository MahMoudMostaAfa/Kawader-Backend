using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Commands.CancelSubscription
{
    public class CancelSubscriptionCommandValidator : AbstractValidator<CancelSubscriptionCommand>
    {
        public CancelSubscriptionCommandValidator()
        {
            RuleFor(x => x.SubscriptionId).NotNull().WithMessage("The Subscription Id is required")
                .NotEqual(Guid.Empty).WithMessage("The Subscription Id can't be empty");
        }
    }
}
