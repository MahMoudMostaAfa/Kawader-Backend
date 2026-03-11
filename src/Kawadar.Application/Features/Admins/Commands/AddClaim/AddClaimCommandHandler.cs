using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Domain.Common.Constants;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Admins.Commands.AddClaim
{
    public class AddClaimCommandHandler(IUser user, IIdentityService identityService) : IRequestHandler<AddClaimCommand, Result<Success>>
    {
        public async Task<Result<Success>> Handle(AddClaimCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            if (!Permissions.GetAllPermissions().Contains(request.permission)) return Error.NotFound("Permission.NotFound", "This permission doesn't exist");

            var userResult = await identityService.GetUserByUserNameAsync(request.userName);
            if (userResult.IsError) return userResult.Errors;

            var hasAdminRoleResult = await identityService.IsInRoleAsync(userResult.Value.Id, DefaultRoles.Admin);
            if (!hasAdminRoleResult.Value) return Error.Conflict("Conflict", "You can only add permissions to admins");

            var AddClaimResult = await identityService.AddClaimAsync(userResult.Value.Id, "Permission", request.permission);
            return Result.Success;
        }
    }
}
