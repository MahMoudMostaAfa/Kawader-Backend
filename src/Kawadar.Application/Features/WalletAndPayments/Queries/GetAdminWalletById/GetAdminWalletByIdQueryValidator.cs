using FluentValidation;

namespace Kawadar.Application.Features.WalletAndPayments.Queries.GetAdminWalletById;

public class GetAdminWalletByIdQueryValidator : AbstractValidator<GetAdminWalletByIdQuery>
{
  public GetAdminWalletByIdQueryValidator()
  {
    RuleFor(x => x.WalletId)
      .NotEmpty().WithMessage("Wallet ID is required.");
  }
}
