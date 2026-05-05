using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Commands.CreateSubscriptionPlan
{
    public class CreateSubscriptionPlanCommandValidator : AbstractValidator<CreateSubscriptionPlanCommand>
    {
        public CreateSubscriptionPlanCommandValidator()
        {
            RuleFor(x => x.name).NotNull().WithMessage("The subscription plan name is required")
                .NotEmpty().WithMessage("The subscription plan can't be empty");

            RuleFor(x => x.CycleType).IsInEnum();

            RuleFor(x => x.proposalsPerMonth).GreaterThanOrEqualTo(30).WithMessage("The proposals must be greater than or equal to 30");

            RuleFor(x => x.TotalPortfolioProjects).GreaterThanOrEqualTo(20).WithMessage("The Portfolio projects must be greater than or equal to 20");
        }
    }
}
