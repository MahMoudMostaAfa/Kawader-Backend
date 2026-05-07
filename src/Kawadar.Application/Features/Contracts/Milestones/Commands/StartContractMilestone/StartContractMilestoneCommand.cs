using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Milestones.Commands.StartContractMilestone;

public record StartContractMilestoneCommand(Guid ContractId, Guid MilestoneId)
  : IRequest<Result<Updated>>;
