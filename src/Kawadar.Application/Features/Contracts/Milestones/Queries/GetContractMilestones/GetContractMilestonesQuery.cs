using Kawadar.Application.Features.Contracts.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Milestones.Queries.GetContractMilestones;

public record GetContractMilestonesQuery(Guid ContractId) : IRequest<Result<List<ContractMilestoneDto>>>;
