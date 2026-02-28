using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Admins.Commands.DeleteUser
{
    public class DeleteUserCommandHandler(IUser user, IUnitOfWork unitOfWork,
        IUsersRepository usersRepository, IIdentityService identityService) : IRequestHandler<DeleteUserCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var userResult = await identityService.GetUserByUserNameAsync(request.userName);
            if (userResult.IsError) return userResult.Errors;

            var userProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userResult.Value.Id);
            if (userProfileResult.IsError) return userProfileResult.Errors;

            var userProfile = userProfileResult.Value;
            var deleteResult = userProfile.Delete();
            if (deleteResult.IsError) return deleteResult.Errors;

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Deleted;
        }
    }
}
