using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Commands.CancelContract;

public record CancelContractCommand(Guid ContractId) : IRequest<Result<Updated>>;