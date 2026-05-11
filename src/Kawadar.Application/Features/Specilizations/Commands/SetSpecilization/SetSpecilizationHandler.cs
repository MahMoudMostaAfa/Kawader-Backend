using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Specilizations.Commands.SetSpecilization
{
    public class SetSpecilizationHandler(IUser user, ISpecilizationRepository specilizationRepository
        , IUsersRepository usersRepository, IUnitOfWork unitOfWork, IRecommendationService recommendationService, ISkillRepository skillRepository) : IRequestHandler<SetSpecilizationCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(SetSpecilizationCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var userProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
            if (userProfileResult.IsError) return userProfileResult.Errors;

            var userProfile = userProfileResult.Value;

            var specilizationResult = await specilizationRepository.GetByName(request.specilizationName);
            if (specilizationResult.IsError) return specilizationResult.Errors;

            userProfile.updateSpecilization(specilizationResult.Value.Id);
            await unitOfWork.SaveChangesAsync();

            // Update user labels in Gorse with new specialization
            var skills = await skillRepository.GetFreelancerSkillsByUserProfileId(userProfile.Id);
            var labels = skills
                .Concat(new[] { specilizationResult.Value.Name.ToLower(), userProfile.ExperienceYear.ToString().ToLower(), userProfile.ProfileType.ToString().ToLower() })
                .ToArray();

            await recommendationService.UpdateUserAsync(
                userProfile.Id,
                labels: labels,
                comment: userProfile.FullName,
                ct: cancellationToken);

            return Result.Updated;
        }
    }
}
