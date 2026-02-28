using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Admins.Commands.BanUser
{
    public class BanUserCommandHandler(IUser user, IUsersRepository usersRepository
        , IUnitOfWork unitOfWork, IIdentityService identityService) : IRequestHandler<BanUserCommand, Result<Success>>
    {
        public async Task<Result<Success>> Handle(BanUserCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var userResult = await identityService.GetUserByUserNameAsync(request.userName);
            if (userResult.IsError) return userResult.Errors;

            var userProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userResult.Value.Id);
            if (userProfileResult.IsError) return userProfileResult.Errors;

            var userProfile = userProfileResult.Value;

            var banResult = userProfile.Ban(request.BannedUntil);
            if (banResult.IsError) return banResult.Errors;

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success;
        }
    }
}
