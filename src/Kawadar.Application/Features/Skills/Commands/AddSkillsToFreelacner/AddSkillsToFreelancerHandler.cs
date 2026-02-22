using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Skills.FreelancerSkill;
using Kawadar.Domain.Skills.FreelancerSkill.Enum;
using MediatR;

namespace Kawadar.Application.Features.Skills.Commands.AddSkillsToFreelacner
{
    public class AddSkillsToFreelancerHandler(IUser user, IUsersRepository usersRepository
        , ISkillRepository skillRepository, IUnitOfWork unitOfWork) : IRequestHandler<AddSkillsToFreelacnerCommand, Result<List<FreelancerSkill>>>
    {
        public async Task<Result<List<FreelancerSkill>>> Handle(AddSkillsToFreelacnerCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var userProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (userProfileResult.IsError) return userProfileResult.Errors;

            var userProfile = userProfileResult.Value;
            List<FreelancerSkill> skills = new();

            foreach(var skill in request.Skills)
            {
                if(skill.SkillType == SkillType.Predefined)
                {
                    var exists = await skillRepository.getByIdAsync(skill.SkillId.Value);
                    if (exists.IsError) return exists.Errors;
                }
                var freelancerSkillResult = FreelancerSkill.Create(userProfile.Id, skill.SkillId, skill.SkillType, skill.CustomSkillName);
                if (freelancerSkillResult.IsError) return freelancerSkillResult.Errors;
                skills.Add(freelancerSkillResult.Value);
            }

            var addResult = await skillRepository.addSkillToFreelacner(skills);
            if (addResult.IsError) return addResult.Errors;

            await unitOfWork.SaveChangesAsync();
            return skills;
        }
    }
}
