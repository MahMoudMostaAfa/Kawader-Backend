using FluentValidation;
using Kawadar.Application.Common.Constants;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Commands.UpdateSubscriptionPlan
{
    public class UpdateSuscriptionPlanCommandValidator : AbstractValidator<UpdateSubscriptionPlanCommand>
    {
        public UpdateSuscriptionPlanCommandValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("The Subscription Id is required")
                .NotEqual(Guid.Empty).WithMessage("The Subscription Id can't be empty");

            RuleFor(x => x.ProposalsPerMonth).GreaterThan(FreePlanFeatures.ProposalsPerMont);

            RuleFor(x => x.PortfolioProjects).GreaterThan(FreePlanFeatures.PortfolioProjects);
        }
    }
}
