using FluentValidation;
using Kawadar.Domain.WalletAndPayments.Payouts;
using Kawadar.Domain.WalletAndPayments.Payouts.Enums;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.AddPayoutAccount;

public class AddPayoutAccountCommandValidator : AbstractValidator<AddPayoutAccountCommand>
{
  public AddPayoutAccountCommandValidator()
  {
    RuleFor(c => c.PayoutType)
      .IsInEnum().WithMessage("Invalid payout type.");

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

      switch (command.PayoutType)
      {
        case PayoutType.MobileWallet:
          if (command.AccountDetails is not MobileWalletAccountDetails mobile)
          {
            context.AddFailure("AccountDetails", "Account details must be MobileWalletAccountDetails for MobileWallet payout type.");
            return;
          }

          ValidateMobileWallet(mobile, context);
          break;

        case PayoutType.BankTransfer:
          if (command.AccountDetails is not BankTransferAccountDetails bank)
          {
            context.AddFailure("AccountDetails", "Account details must be BankTransferAccountDetails for BankTransfer payout type.");
            return;
          }

          ValidateBankTransfer(bank, context);
          break;

        case PayoutType.InstaPay:
          if (command.AccountDetails is not InstaPayAccountDetails instaPay)
          {
            context.AddFailure("AccountDetails", "Account details must be InstaPayAccountDetails for InstaPay payout type.");
            return;
          }

          ValidateInstaPay(instaPay, context);
          break;
      }
    });
  }

  private static void ValidateMobileWallet(MobileWalletAccountDetails details, ValidationContext<AddPayoutAccountCommand> context)
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

  private static void ValidateBankTransfer(BankTransferAccountDetails details, ValidationContext<AddPayoutAccountCommand> context)
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

  private static void ValidateInstaPay(InstaPayAccountDetails details, ValidationContext<AddPayoutAccountCommand> context)
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
