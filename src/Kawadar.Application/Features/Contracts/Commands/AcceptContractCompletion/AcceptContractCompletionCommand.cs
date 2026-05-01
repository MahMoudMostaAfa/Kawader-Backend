using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Commands.AcceptContractCompletion;

public record AcceptContractCompletionCommand(Guid ContractId) :
  IRequest<Result<Updated>>;