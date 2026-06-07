using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Queries.GetUserSubscriptionByUserProfileId
{
    public class GetUserSubscriptionsByUserProfileIdValidation : AbstractValidator<GetUserSubscriptionByUserProfileIdQuery>
    {
        public GetUserSubscriptionsByUserProfileIdValidation()
        {
            RuleFor(x => x.status).IsInEnum().When(x => x.status is not null);

            RuleFor(x => x.page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.pageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);

            RuleFor(x => x.sortBy)
              .Must(s => s is "newest" or "oldest")
              .WithMessage("SortBy must be 'newest' or 'oldest'.");
        }
    }
}
