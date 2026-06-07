using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Violations.Commands.SolveViolation
{
    public class SolveViolationCommandHandler(IUser user, IViolationRepository violationRepository,
        IUsersRepository usersRepository, IUnitOfWork unitOfWork) : IRequestHandler<SolveViolationCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(SolveViolationCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var UserProfile = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (UserProfile.IsError) return UserProfile.Errors;

            var violation = await violationRepository.GetViolationById(request.Id);
            if (violation.IsError) return violation.Errors;

            violation.Value.Solve(request.status, request.action, request.noteByAdmin, UserProfile.Value.Id);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Updated;
        }
    }
}
