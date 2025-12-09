
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Specilizations.Commands.UpdateSpecilization
{
    public record UpdateSpecilizationCommand(Guid Id, string name, bool isActive) : IRequest<Result<Updated>>;
}
