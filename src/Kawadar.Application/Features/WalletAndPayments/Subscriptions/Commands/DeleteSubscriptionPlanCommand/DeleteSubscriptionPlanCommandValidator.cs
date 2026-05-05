using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Commands.DeleteSubscriptionPlanCommand
{
    public class DeleteSubscriptionPlanCommandValidator : AbstractValidator<DeleteSubscriptionPlanCommand>
    {
        public DeleteSubscriptionPlanCommandValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("The Subscription Id is required")
                .NotEqual(Guid.Empty).WithMessage("The subscription Id can't be empty");
        }
    }
}
