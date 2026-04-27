using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Violations.Enums;
using MediatR;

namespace Kawadar.Application.Features.Violations.Commands.SolveViolation
{
    public record SolveViolationCommand(Guid Id, ViolationStatus status, string action, string noteByAdmin) : IRequest<Result<Updated>>;
}
