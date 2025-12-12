using Kawadar.Application.Features.Specilizations.DTO;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Specilizations.Commands.CreateSpecilization
{
    public record CreateSpecilizationCommand(string name, bool isActive) : IRequest<Result<SpecilizationDTO>>;
}
