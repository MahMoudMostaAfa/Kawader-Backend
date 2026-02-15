using Kawadar.Application.Features.Badges.DTOs;
using Kawadar.Domain.Common.Results;
using Microsoft.AspNetCore.Http;
using MediatR;

namespace Kawadar.Application.Features.Badges.Commands.CreateBadge
{
    public record CreateBadgeCommand(string title, IFormFile Icon, string description): IRequest<Result<BadgeDTO>>;
}
