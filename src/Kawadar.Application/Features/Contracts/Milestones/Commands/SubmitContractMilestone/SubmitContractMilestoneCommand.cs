using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Milestones.Commands.SubmitContractMilestone;

public record SubmitContractMilestoneCommand(Guid ContractId, Guid MilestoneId)
  : IRequest<Result<Updated>>;
