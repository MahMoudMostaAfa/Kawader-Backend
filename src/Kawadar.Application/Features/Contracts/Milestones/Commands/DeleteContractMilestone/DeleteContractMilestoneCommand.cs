using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Milestones.Commands.DeleteContractMilestone;

public record DeleteContractMilestoneCommand(Guid ContractId, Guid MilestoneId) : IRequest<Result<Updated>>;
