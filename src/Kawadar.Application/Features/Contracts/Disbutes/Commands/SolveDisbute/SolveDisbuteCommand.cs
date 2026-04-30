using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Contracts.Disbutes.Enum;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Disbutes.Commands.SolveDisbute
{
    public record SolveDisbuteCommand(Guid DisbuteId, DisbuteStatus status, string? resolution) : IRequest<Result<Updated>>;
}
