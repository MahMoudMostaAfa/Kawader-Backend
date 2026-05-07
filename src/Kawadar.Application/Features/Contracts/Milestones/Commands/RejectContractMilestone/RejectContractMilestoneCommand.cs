using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Milestones.Commands.RejectContractMilestone;

public record RejectContractMilestoneCommand(Guid ContractId, Guid MilestoneId, string? Reason)
  : IRequest<Result<Updated>>;
