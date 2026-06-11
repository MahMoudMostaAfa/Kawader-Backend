using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Proposals.Dtos;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Proposals.Enums;
using MediatR;

namespace Kawadar.Application.Features.Proposals.Queries.GetProposals;

public record GetProposalsQuery(
    Guid JobId,
    JobProposalType? Type,
    JobProposalStatus? Status,
    int Page = 1,
    int PageSize = 10,
    string DatesortBy = "newest",
    string? PriceSortBy = null,
    string? EstimatedTimeSortBy = null
) : IRequest<Result<PaginatedList<ProposalSummaryDto>>>, ICachedQuery
{
    public string CacheKey => $"Proposals-{JobId}-{Type?.ToString() ?? "all"}-{Status?.ToString() ?? "all"}-{Page}-{PageSize}-{DatesortBy}-{PriceSortBy ?? "none"}-{EstimatedTimeSortBy ?? "none"}";

    public string[] Tags => ["proposals"];
}
