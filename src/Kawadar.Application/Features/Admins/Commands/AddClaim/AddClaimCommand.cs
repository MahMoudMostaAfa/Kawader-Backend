using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Admins.Commands.AddClaim
{
    public record AddClaimCommand(string userName, string permission) : IRequest<Result<Success>>;
}