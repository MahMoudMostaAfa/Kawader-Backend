using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetAdminWallets;

public class GetAdminWalletsQueryValidator : AbstractValidator<GetAdminWalletsQuery>
{
  public GetAdminWalletsQueryValidator()
  {
    RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
    RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
    RuleFor(x => x.MaxBalance)
      .GreaterThanOrEqualTo(x => x.MinBalance)
      .When(x => x.MinBalance.HasValue && x.MaxBalance.HasValue)
      .WithMessage("MaxBalance must be greater than or equal to MinBalance.");
  }
}
