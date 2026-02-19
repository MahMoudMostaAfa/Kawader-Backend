using Kawadar.Application.Features.Badges.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Badges.Queries.GetFreelancerBadgesQuery
{
    public record GetFreelancerBadgesQuery: IRequest<Result<List<BadgeDTO>>>;
}
