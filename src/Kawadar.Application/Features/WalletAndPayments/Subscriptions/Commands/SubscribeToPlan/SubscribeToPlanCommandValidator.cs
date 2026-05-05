using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Commands.SubscribeToPlan
{
    public class SubscribeToPlanCommandValidator : AbstractValidator<SubscribeToPlanCommand>
    {
        public SubscribeToPlanCommandValidator()
        {
            RuleFor(x => x.PlanId).NotNull().WithMessage("The plan Id is required")
                .NotEqual(Guid.Empty).WithMessage("The plan Id can't be empty");
        }
    }
}
