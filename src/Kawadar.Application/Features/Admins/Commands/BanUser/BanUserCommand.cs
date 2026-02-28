using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Admins.Commands.BanUser
{
    public record BanUserCommand(string userName, DateTime BannedUntil) : IRequest<Result<Success>>;
}
