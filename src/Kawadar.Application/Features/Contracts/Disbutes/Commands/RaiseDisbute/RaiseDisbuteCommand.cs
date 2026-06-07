using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Disbutes.Commands.RaiseDisbute
{
    public record RaiseDisbuteCommand(Guid ContractId, string reason) : IRequest<Result<Created>>;
}
