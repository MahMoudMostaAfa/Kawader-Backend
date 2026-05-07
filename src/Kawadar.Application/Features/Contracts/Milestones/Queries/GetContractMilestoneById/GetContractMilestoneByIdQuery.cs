using Kawadar.Application.Features.Contracts.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Milestones.Queries.GetContractMilestoneById;

public record GetContractMilestoneByIdQuery(Guid ContractId, Guid MilestoneId) : IRequest<Result<ContractMilestoneDto>>;
