using Kawadar.Application.Features.Admins.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Admins.Queries.GetReviewsStatistics
{
    public record GetReviewStatisticsQuery() : IRequest<Result<ReviewStatisticsDto>>;
}
