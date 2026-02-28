using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Constants;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles;
using Kawadar.Domain.UserProfiles.Enums;
using MediatR;

namespace Kawadar.Application.Features.Admins.Commands.CreateAdmin
{
    public class CreateAdminCommandHandler(IUser user, IUsersRepository usersRepository
        ,IIdentityService identityService, IUnitOfWork unitOfWork) : IRequestHandler<CreateAdminCommand, Result<Success>>
    {
        public async Task<Result<Success>> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var generateUserNameResult = await identityService.GenerateUserNameAsync(request.FirstName, request.LastName);
            if (generateUserNameResult.IsError) return Error.Failure("User.GenerateUserNameFailed", "Failed to generate username.");

            var createUserResult = await identityService.RegisterAsync(request.Email, generateUserNameResult.Value, request.Password);
            if (createUserResult.IsError) return createUserResult.Errors;

            var AdminId = createUserResult.Value.Id;

            await identityService.AddToRoleAsync(AdminId, DefaultRoles.Admin);

            var userProfileResult = UserProfile.create(AdminId, request.FirstName, request.LastName, ProfileType.Admin);
            if (userProfileResult.IsError)
            {
                await identityService.DeleteUserAsync(AdminId);
                return userProfileResult.Errors;
            }

            var CreateProfileResult = await usersRepository.CreateUserProfileAsync(userProfileResult.Value);
            if (CreateProfileResult.IsError)
            {
                await identityService.DeleteUserAsync(AdminId);
                return CreateProfileResult.Errors;
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success;
        }
    }
}
