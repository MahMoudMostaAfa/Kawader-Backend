using Kawadar.Application.Features.Proposals.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Proposals.Queries.GetProposalById;

public record GetProposalByIdQuery(Guid ProposalId) : IRequest<Result<ProposalDetailsDto>>;