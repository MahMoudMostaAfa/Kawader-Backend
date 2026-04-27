using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Contracts.Enums;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Commands.CreateContract;

public record CreateContractCommand(
  Guid JobId,
  Guid ProposaslId,
  ContractType ContractType,
  DateTime StartDate
) : IRequest<Result<Guid>>;