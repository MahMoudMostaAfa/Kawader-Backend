using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Contracts.Disbutes.Dtos;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Contracts.Disbutes.Enum;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Disbutes.Queries.GetDisbutes
{
    public record GetDisbutesQuery(DisbuteStatus? status, int Page = 1, int PageSize = 10, string SortBy = "newest") : IRequest<Result<PaginatedList<BriefDisbuteDto>>>;
}
