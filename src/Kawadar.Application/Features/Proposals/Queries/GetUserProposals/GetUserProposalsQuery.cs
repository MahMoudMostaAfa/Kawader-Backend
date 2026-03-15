using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Proposals.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Proposals.Queries.GetUserProposals;


public record GetUserProposalsQuery(string SortBy, int PageSize, int PageNumber) : IRequest<Result<PaginatedList<ProposalSummaryDto>>>;