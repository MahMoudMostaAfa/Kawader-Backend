using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles.UserReports;
using MediatR;

namespace Kawadar.Application.Features.ProfileManagment.Commands.ReportUser
{
    public class ReportUserCommandHandler(IUser user, IUnitOfWork unitOfWork, IUsersRepository usersRepository,
        IIdentityService identityService) : IRequestHandler<ReportUserCommand, Result<Created>>
    {
        public async Task<Result<Created>> Handle(ReportUserCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var reporterUserProfile = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (reporterUserProfile.IsError) return reporterUserProfile.Errors;

            var reportedUser = await identityService.GetUserByUserNameAsync(request.ReportedUserName);
            if (reportedUser.IsError) return reportedUser.Errors;

            var reportedUserProfile = await usersRepository.GetUserProfileByUserIdAsync(reportedUser.Value.Id);
            if (reportedUserProfile.IsError) return reportedUserProfile.Errors;

            if (reportedUserProfile.Value.Id == reporterUserProfile.Value.Id) return Error.Conflict("You can't report yourself");

            var userReport = UserReport.Create(reportedUserProfile.Value.Id,
                reporterUserProfile.Value.Id, request.reportType, request.content);
            if (userReport.IsError) return userReport.Errors;

            await usersRepository.AddUserReport(userReport.Value);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Created;
        }
    }
}
