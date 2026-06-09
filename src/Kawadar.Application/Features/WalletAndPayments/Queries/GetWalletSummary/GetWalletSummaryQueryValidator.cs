using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetWalletSummary;

public class GetWalletSummaryQueryValidator : AbstractValidator<GetWalletSummaryQuery>
{
    public GetWalletSummaryQueryValidator()
    {
        RuleFor(x => x.Type).IsInEnum().When(x => x.Type is not null);

        RuleFor(x => x.Status).IsInEnum().When(x => x.Status is not null);

        RuleFor(x => x.ReferenceType).IsInEnum().When(x => x.ReferenceType is not null);

        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);

        RuleFor(x => x.SortBy)
          .Must(s => s is "newest" or "oldest")
          .WithMessage("SortBy must be 'newest' or 'oldest'.");
    }
}
