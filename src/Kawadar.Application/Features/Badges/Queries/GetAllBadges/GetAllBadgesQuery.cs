using Kawadar.Application.Features.Badges.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Badges.Queries.GetAllBadges
{
    public record GetAllBadgesQuery() : IRequest<Result<List<BadgeDTO>>>;
}
