using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Badges.Commands.CreateBadge
{
    public record CreateBadgeCommand(string title, string IconUrl, string description): IRequest<Result<Success>>;
}
