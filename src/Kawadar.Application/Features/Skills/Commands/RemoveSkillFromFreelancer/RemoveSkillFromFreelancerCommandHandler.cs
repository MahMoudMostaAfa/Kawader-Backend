using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Skills.Commands.RemoveSkillFromFreelancer
{
    public class RemoveSkillFromFreelancerCommandHandler(IUser user, ISkillRepository skillRepository
        , IUsersRepository usersRepository, IUnitOfWork unitOfWork) : IRequestHandler<RemoveSkillFromFreelancerCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(RemoveSkillFromFreelancerCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var userProfile = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (userProfile.IsError) return userProfile.Errors;

            var deleteResult = await skillRepository.RemoveSkillFromFreelancer(request.skillName, userProfile.Value.Id);
            if (deleteResult.IsError) return deleteResult.Errors;

            await unitOfWork.SaveChangesAsync();
            return Result.Deleted;
        }
    }
}
