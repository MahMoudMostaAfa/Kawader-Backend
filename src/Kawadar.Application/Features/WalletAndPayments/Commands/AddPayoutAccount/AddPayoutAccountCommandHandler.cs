using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.WalletAndPayments.Payouts;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.AddPayoutAccount;

public class AddPayoutAccountCommandHandler : IRequestHandler<AddPayoutAccountCommand, Result<Guid>>
{
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IUserPayoutAccountRepository _payoutAccountRepository;
  private readonly IUnitOfWork _unitOfWork;

  public AddPayoutAccountCommandHandler(IUser user, IUsersRepository usersRepository,
    IUserPayoutAccountRepository payoutAccountRepository, IUnitOfWork unitOfWork)
  {
    _user = user;
    _usersRepository = usersRepository;
    _payoutAccountRepository = payoutAccountRepository;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<Guid>> Handle(AddPayoutAccountCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    // If this account should be default, clear all existing defaults first
    if (request.IsDefault)
    {
      var existingDefaultsResult = await _payoutAccountRepository.GetAllDefaultsByUserIdAsync(userProfile.Id, cancellationToken);
      if (existingDefaultsResult.IsSuccess)
      {
        foreach (var existingDefault in existingDefaultsResult.Value)
        {
          existingDefault.ClearDefault();
        }
      }
    }

    var accountDetailsJson = System.Text.Json.JsonSerializer.Serialize(request.AccountDetails, request.AccountDetails.GetType());

    var accountResult = UserPayoutAccount.Create(
      userProfile.Id,
      request.PayoutType,
      request.DisplayName,
      accountDetailsJson,
      request.IsDefault
    );

    if (accountResult.IsError) return accountResult.Errors;

    var account = accountResult.Value;
    _payoutAccountRepository.Add(account);

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return account.Id;
  }
}
