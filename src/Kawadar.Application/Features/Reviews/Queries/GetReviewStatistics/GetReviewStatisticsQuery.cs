using Kawadar.Application.Features.Reviews.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Reviews.Queries.GetReviewStatistics
{
    public record GetReviewStatisticsQuery(string userName) : IRequest<Result<ReviewStatisticsDto>>;
}
