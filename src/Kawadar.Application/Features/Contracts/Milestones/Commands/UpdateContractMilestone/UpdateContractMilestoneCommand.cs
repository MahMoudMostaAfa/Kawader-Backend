using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Milestones.Commands.UpdateContractMilestone;

public record UpdateContractMilestoneCommand(
  Guid ContractId,
  Guid MilestoneId,
  DateTime DueDate
) : IRequest<Result<Updated>>;
