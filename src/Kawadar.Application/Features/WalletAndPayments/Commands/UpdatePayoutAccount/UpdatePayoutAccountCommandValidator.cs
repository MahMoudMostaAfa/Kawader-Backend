using FluentValidation;
using Kawadar.Domain.WalletAndPayments.Payouts;
using Kawadar.Domain.WalletAndPayments.Payouts.Enums;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.UpdatePayoutAccount;

public class UpdatePayoutAccountCommandValidator : AbstractValidator<UpdatePayoutAccountCommand>
{
  public UpdatePayoutAccountCommandValidator()
  {
    RuleFor(c => c.AccountId)
      .NotEmpty().WithMessage("Account ID is required.");

    RuleFor(c => c.DisplayName)
      .NotEmpty().WithMessage("Display name is required.")
      .MaximumLength(100).WithMessage("Display name must not exceed 100 characters.");

    RuleFor(c => c.AccountDetails)
      .NotNull().WithMessage("Account details are required.");

    RuleFor(c => c).Custom((command, context) =>
    {
      if (command.AccountDetails is null)
      {
        return;
      }

      switch (command.AccountDetails)
      {
        case MobileWalletAccountDetails mobile:
          ValidateMobileWallet(mobile, context);
          break;
        case BankTransferAccountDetails bank:
          ValidateBankTransfer(bank, context);
          break;
        case InstaPayAccountDetails instaPay:
          ValidateInstaPay(instaPay, context);
          break;
        default:
          context.AddFailure("AccountDetails", "Account details must be a supported payout account details type.");
          break;
      }
    });
  }

  private static void ValidateMobileWallet(MobileWalletAccountDetails details, ValidationContext<UpdatePayoutAccountCommand> context)
  {
    if (string.IsNullOrWhiteSpace(details.PhoneNumber))
    {
      context.AddFailure("AccountDetails.PhoneNumber", "Phone number is required.");
      return;
    }

    if (details.PhoneNumber.Length != 11)
    {
      context.AddFailure("AccountDetails.PhoneNumber", "Phone number must be 11 digits.");
      return;
    }

    if (!details.PhoneNumber.All(char.IsDigit))
    {
      context.AddFailure("AccountDetails.PhoneNumber", "Phone number must contain only digits.");
      return;
    }

    var expectedPrefix = details.Provider switch
    {
      MobileWalletProvider.VodafoneCash => "010",
      MobileWalletProvider.OrangeMoney => "012",
      MobileWalletProvider.EtisalatCash => "011",
      _ => null
    };

    if (expectedPrefix is not null && !details.PhoneNumber.StartsWith(expectedPrefix))
    {
      context.AddFailure("AccountDetails.PhoneNumber", $"Phone number must start with {expectedPrefix} for {details.Provider}.");
    }
  }

  private static void ValidateBankTransfer(BankTransferAccountDetails details, ValidationContext<UpdatePayoutAccountCommand> context)
  {
    if (string.IsNullOrWhiteSpace(details.BankName))
    {
      context.AddFailure("AccountDetails.BankName", "Bank name is required.");
    }

    if (string.IsNullOrWhiteSpace(details.AccountHolderName))
    {
      context.AddFailure("AccountDetails.AccountHolderName", "Account holder name is required.");
    }

    if (string.IsNullOrWhiteSpace(details.AccountNumber))
    {
      context.AddFailure("AccountDetails.AccountNumber", "Account number is required.");
    }
  }

  private static void ValidateInstaPay(InstaPayAccountDetails details, ValidationContext<UpdatePayoutAccountCommand> context)
  {
    if (string.IsNullOrWhiteSpace(details.IPA))
    {
      context.AddFailure("AccountDetails.IPA", "InstaPay address is required.");
      return;
    }

    if (!details.IPA.Contains('@'))
    {
      context.AddFailure("AccountDetails.IPA", "InstaPay address must contain '@'.");
    }
  }
}
