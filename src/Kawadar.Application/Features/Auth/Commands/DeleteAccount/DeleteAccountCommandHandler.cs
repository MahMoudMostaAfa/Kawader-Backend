using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Auth.Commands.DeleteAccount;

public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, Result<Deleted>>
{
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IUnitOfWork _unitOfWork;
  private readonly IAccountDeletionScheduler _accountDeletionScheduler;

  public DeleteAccountCommandHandler(
    IUser user,
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork,
    IAccountDeletionScheduler accountDeletionScheduler)
  {
    _user = user;
    _usersRepository = usersRepository;
    _unitOfWork = unitOfWork;
    _accountDeletionScheduler = accountDeletionScheduler;
  }

  public async Task<Result<Deleted>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var profileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (profileResult.IsError) return profileResult.Errors;

    var profile = profileResult.Value;

    var markResult = profile.MarkAsDeleted();
    if (markResult.IsError) return markResult.Errors;

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    // Schedule permanent deletion after 1 month
    _accountDeletionScheduler.SchedulePermanentDeletion(userId, TimeSpan.FromDays(30));

    return Result.Deleted;
  }
}
