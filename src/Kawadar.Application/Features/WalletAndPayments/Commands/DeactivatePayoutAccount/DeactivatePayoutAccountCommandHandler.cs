using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.WalletAndPayments.Commands.DeactivatePayoutAccount;

public class DeactivatePayoutAccountCommandHandler : IRequestHandler<DeactivatePayoutAccountCommand, Result<Deleted>>
{
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IUserPayoutAccountRepository _payoutAccountRepository;
  private readonly IUnitOfWork _unitOfWork;

  public DeactivatePayoutAccountCommandHandler(IUser user, IUsersRepository usersRepository,
    IUserPayoutAccountRepository payoutAccountRepository, IUnitOfWork unitOfWork)
  {
    _user = user;
    _usersRepository = usersRepository;
    _payoutAccountRepository = payoutAccountRepository;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<Deleted>> Handle(DeactivatePayoutAccountCommand request, CancellationToken cancellationToken)
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

    var deactivateResult = account.Deactivate();
    if (deactivateResult.IsError) return deactivateResult.Errors;

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Deleted;
  }
}
