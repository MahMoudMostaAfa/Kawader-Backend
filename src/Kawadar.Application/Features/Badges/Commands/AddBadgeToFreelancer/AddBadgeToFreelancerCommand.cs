using Kawadar.Domain.Badges.FreelancerBadges;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Badges.Commands.AddBadgeToFreelancer
{
    public record AddBadgeToFreelancerCommand(Guid FreelancerId, Guid BadgeId): IRequest<Result<FreelancerBadge>>;
}
