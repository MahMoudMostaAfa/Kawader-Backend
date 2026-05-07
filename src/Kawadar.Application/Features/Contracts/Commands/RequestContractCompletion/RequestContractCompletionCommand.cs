using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Commands.RequestContractCompletion;



public record RequesContractCompletionCommand(Guid ContractId) : IRequest<Result<Created>>;