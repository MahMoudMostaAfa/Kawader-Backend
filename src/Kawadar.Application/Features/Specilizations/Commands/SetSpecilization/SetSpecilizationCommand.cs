using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Specilizations.Commands.SetSpecilization
{
    public record SetSpecilizationCommand(string specilizationName): IRequest<Result<Updated>>;
}