using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetMyPayoutAccounts;

public class GetMyPayoutAccountsQueryValidator : AbstractValidator<GetMyPayoutAccountsQuery>
{
  public GetMyPayoutAccountsQueryValidator()
  {
    // No additional validation needed for this query
  }
}
