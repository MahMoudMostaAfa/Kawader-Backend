using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.ProfileManagment.Commands.UpdateUserReport
{
    public class UpdateUserReportCommandHandler(IUser user, IUsersRepository usersRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateUserReportCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateUserReportCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var reportResult = await usersRepository.GetUserReportById(request.reportId);
            if (reportResult.IsError) return reportResult.Errors;

            var report = reportResult.Value;
            report.Update(request.ReportStatus, request.ActionTaken);
            await unitOfWork.SaveChangesAsync();
            return Result.Updated;
        }
    }
}
