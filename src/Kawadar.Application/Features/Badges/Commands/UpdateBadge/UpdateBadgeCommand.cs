using Kawadar.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;


namespace Kawadar.Application.Features.Badges.Commands.UpdateBadge
{
    public record UpdateBadgeCommand(Guid badgeId, IFormFile Icon) : IRequest<Result<Updated>>;
}
