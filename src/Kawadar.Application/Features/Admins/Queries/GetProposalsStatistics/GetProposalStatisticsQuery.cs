using Kawadar.Application.Features.Admins.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Admins.Queries.GetProposalsStatistics
{
    public record GetProposalStatisticsQuery() : IRequest<Result<ProposalStatisticsDto>>;
}
