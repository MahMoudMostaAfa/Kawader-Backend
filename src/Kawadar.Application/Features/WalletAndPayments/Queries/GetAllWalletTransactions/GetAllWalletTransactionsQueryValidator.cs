using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetAllWalletTransactions
{
    public class GetAllWalletTransactionsQueryValidator : AbstractValidator<GetAllWalletTransactionsQuery>
    {
        public GetAllWalletTransactionsQueryValidator()
        {
            RuleFor(x => x.WalletId).NotNull().WithMessage("The wallet Id is required")
                .NotEqual(Guid.Empty).WithMessage("The wallet Id can't be empty");

            RuleFor(x => x.type).IsInEnum().When(x => x.type is not null);

            RuleFor(x => x.status).IsInEnum().When(x => x.status is not null);

            RuleFor(x => x.referenceType).IsInEnum().When(x => x.referenceType is not null);

            RuleFor(x => x.page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.pageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);

            RuleFor(x => x.sortBy)
              .Must(s => s is "newest" or "oldest")
              .WithMessage("SortBy must be 'newest' or 'oldest'.");
        }
    }
}
