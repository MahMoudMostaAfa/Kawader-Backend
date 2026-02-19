using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Badges;
using Kawadar.Domain.Badges.FreelancerBadges;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Badges.Commands.AddBadgeToFreelancer
{
    public class AddBadgeToFreelancerHandler(IUser user, IBadgeRepository badgeRepository
        ,IUsersRepository usersRepository, IUnitOfWork unitOfWork) : IRequestHandler<AddBadgeToFreelancerCommand, Result<FreelancerBadge>>
    {
        public async Task<Result<FreelancerBadge>> Handle(AddBadgeToFreelancerCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var BadgeResult = await badgeRepository.GetById(request.BadgeId);
            if (BadgeResult.IsError) return BadgeResult.Errors;

            var freelancerBadgeResult = FreelancerBadge.Create(request.FreelancerId, request.BadgeId);
            if (freelancerBadgeResult.IsError) return freelancerBadgeResult.Errors;

            var freelancerBadge = freelancerBadgeResult.Value;
            var addResult = await badgeRepository.AddBadgeToFreelancer(freelancerBadge);
            if (addResult.IsError) return addResult.Errors;

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return freelancerBadge;
        }
    }
}
