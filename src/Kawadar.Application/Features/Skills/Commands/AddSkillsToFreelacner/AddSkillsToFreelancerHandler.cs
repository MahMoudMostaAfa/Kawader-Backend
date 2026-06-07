using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Skills.FreelancerSkill;
using Kawadar.Domain.Skills.FreelancerSkill.Enum;
using MediatR;

namespace Kawadar.Application.Features.Skills.Commands.AddSkillsToFreelacner
{
    public class AddSkillsToFreelancerHandler(IUser user, IUsersRepository usersRepository
        , ISkillRepository skillRepository, IUnitOfWork unitOfWork, IRecommendationService recommendationService, ISpecilizationRepository specilizationRepository) : IRequestHandler<AddSkillsToFreelacnerCommand, Result<List<FreelancerSkill>>>
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
                    var exists = await skillRepository.getByIdAsync(skill.SkillId!.Value);
                    if (exists.IsError) return exists.Errors;
                }
                var freelancerSkillResult = FreelancerSkill.Create(userProfile.Id, skill.SkillId, skill.SkillType, skill.CustomSkillName);
                if (freelancerSkillResult.IsError) return freelancerSkillResult.Errors;
                skills.Add(freelancerSkillResult.Value);
            }

            var addResult = await skillRepository.addSkillToFreelacner(skills);
            if (addResult.IsError) return addResult.Errors;

            await unitOfWork.SaveChangesAsync();

            // Update user labels in Gorse with full skill set
            var allSkillNames = await skillRepository.GetFreelancerSkillsByUserProfileId(userProfile.Id);
            var labels = allSkillNames
                .Select(s => s.ToLower())
                .Concat(new[] { userProfile.ExperienceYear.ToString().ToLower(), userProfile.ProfileType.ToString().ToLower() })
                .ToList();

            // Add specialization if set
            if (userProfile.SpecializationId.HasValue)
            {
                var specResult = await specilizationRepository.GetById(userProfile.SpecializationId.Value);
                if (!specResult.IsError)
                    labels.Add(specResult.Value.Name.ToLower());
            }

            await recommendationService.UpdateUserAsync(
                userProfile.Id,
                labels: labels.ToArray(),
                comment: userProfile.FullName,
                ct: cancellationToken);

            return skills;
        }
    }
}
