using Kawadar.Domain.Common.Results;
using MediatR;


namespace Kawadar.Application.Features.Badges.Commands.UpdateBadge
{
    public record UpdateBadgeCommand(Guid badgeId, string IconUrl) : IRequest<Result<Updated>>;
}
