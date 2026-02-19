using Kawadar.Application.Features.Badges.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;


namespace Kawadar.Application.Features.Badges.Queries.GetBadgeById
{
    public record GetBadgeByIdQuery(Guid Id): IRequest<Result<BadgeDTO>>;
}
