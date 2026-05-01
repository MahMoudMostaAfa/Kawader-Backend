using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.UpdatePayoutAccount;

public class UpdatePayoutAccountCommandHandler : IRequestHandler<UpdatePayoutAccountCommand, Result<Updated>>
{
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IUserPayoutAccountRepository _payoutAccountRepository;
  private readonly IUnitOfWork _unitOfWork;

  public UpdatePayoutAccountCommandHandler(IUser user, IUsersRepository usersRepository,
    IUserPayoutAccountRepository payoutAccountRepository, IUnitOfWork unitOfWork)
  {
    _user = user;
    _usersRepository = usersRepository;
    _payoutAccountRepository = payoutAccountRepository;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<Updated>> Handle(UpdatePayoutAccountCommand request, CancellationToken cancellationToken)
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

    // If this account is not active, prevent updates
    if (!account.IsActive)
      return Error.Conflict("PayoutAccount.Inactive", "Cannot update an inactive payout account.");

    // If setting as default, clear all existing defaults first
    if (request.IsDefault)
    {
      var existingDefaultsResult = await _payoutAccountRepository.GetAllDefaultsByUserIdAsync(userProfile.Id, cancellationToken);
      if (existingDefaultsResult.IsSuccess)
      {
        foreach (var existingDefault in existingDefaultsResult.Value)
        {
          if (existingDefault.Id != account.Id)
            existingDefault.ClearDefault();
        }
      }
    }

    var accountDetailsJson = System.Text.Json.JsonSerializer.Serialize(request.AccountDetails, request.AccountDetails.GetType());

    var updateResult = account.Update(request.DisplayName, accountDetailsJson, request.IsDefault);
    if (updateResult.IsError) return updateResult.Errors;

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Updated;
  }
}
