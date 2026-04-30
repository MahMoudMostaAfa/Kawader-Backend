using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Commands.RejectContractCompletion;

public record RejectContractCompletionCommand(Guid ContractId, string Reason) : IRequest<Result<Updated>>;