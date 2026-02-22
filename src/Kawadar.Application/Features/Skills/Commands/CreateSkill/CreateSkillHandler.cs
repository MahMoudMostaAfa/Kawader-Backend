using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Skills;
using MediatR;

namespace Kawadar.Application.Features.Skills.Commands.CreateSkill
{
    public class CreateSkillHandler(IUser user, IUsersRepository usersRepository,
        ISkillRepository skillRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateSkillCommand, Result<Skill>>
    {
        public async Task<Result<Skill>> Handle(CreateSkillCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var userProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (userProfileResult.IsError) return userProfileResult.Errors;

            var existResult = await skillRepository.getByNameAsync(request.name);
            if (existResult.IsSuccess) return Error.Conflict("Skill.AlreadyExists", "A skill with this name already exists");

            var userProfile = userProfileResult.Value;
            var skillResult = Skill.Create(request.name, request.isActive, userProfile.Id);
            if (skillResult.IsError) return skillResult.Errors;

            await skillRepository.addAsync(skillResult.Value);
            await unitOfWork.SaveChangesAsync();
            return skillResult.Value;
        }
    }
}
