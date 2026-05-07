using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.SetDefaultPayoutAccount;

public class SetDefaultPayoutAccountCommandHandler : IRequestHandler<SetDefaultPayoutAccountCommand, Result<Updated>>
{
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IUserPayoutAccountRepository _payoutAccountRepository;
  private readonly IUnitOfWork _unitOfWork;

  public SetDefaultPayoutAccountCommandHandler(IUser user, IUsersRepository usersRepository,
    IUserPayoutAccountRepository payoutAccountRepository, IUnitOfWork unitOfWork)
  {
    _user = user;
    _usersRepository = usersRepository;
    _payoutAccountRepository = payoutAccountRepository;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<Updated>> Handle(SetDefaultPayoutAccountCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var accountResult = await _payoutAccountRepository.GetByIdAsync(request.AccountId, cancellationToken);
    if (accountResult.IsError) return accountResult.Errors;
    var account = accountResult.Value;

    // Ensure the account belongs to the authenticated user
    if (account.UserId != userProfile.Id) return ApplicationErrors.UnauthorizedAccess;

    // Cannot set an inactive account as default
    if (!account.IsActive)
      return Error.Conflict("PayoutAccount.Inactive", "Cannot set an inactive payout account as default.");

    // Clear all existing defaults for this user
    var existingDefaultsResult = await _payoutAccountRepository.GetAllDefaultsByUserIdAsync(userProfile.Id, cancellationToken);
    if (existingDefaultsResult.IsSuccess)
    {
      foreach (var existingDefault in existingDefaultsResult.Value)
      {
        existingDefault.ClearDefault();
      }
    }

    // Set the requested account as default
    var setDefaultResult = account.SetAsDefault();
    if (setDefaultResult.IsError) return setDefaultResult.Errors;

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Updated;
  }
}
