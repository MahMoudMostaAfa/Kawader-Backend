using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Milestones.Commands.ApproveContractMilestone;

public record ApproveContractMilestoneCommand(Guid ContractId, Guid MilestoneId)
  : IRequest<Result<Updated>>;
