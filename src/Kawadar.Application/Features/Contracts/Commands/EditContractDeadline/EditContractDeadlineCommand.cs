using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Commands.EditContractDeadline;


public record EditContractDeadlineCommand(Guid ContractId, DateTime NewDeadline) : IRequest<Result<Updated>>;