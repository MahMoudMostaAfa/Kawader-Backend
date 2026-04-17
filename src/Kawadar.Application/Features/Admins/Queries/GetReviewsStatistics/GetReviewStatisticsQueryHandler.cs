using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Admins.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Admins.Queries.GetReviewsStatistics
{
    public class GetReviewStatisticsQueryHandler(IUser user, IReviewRepository reviewRepository) : IRequestHandler<GetReviewStatisticsQuery, Result<ReviewStatisticsDto>>
    {
        public async Task<Result<ReviewStatisticsDto>> Handle(GetReviewStatisticsQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var reviewDistribution = await reviewRepository.GetReviewDistribution();

            var totalReviewsNumber = reviewDistribution.Value.Values.Sum();
            var averageReviewScore = await reviewRepository.GetAverageReviewScore();

            var ReviewStatisticsDto = new ReviewStatisticsDto
            {
                AverageReviewScore = averageReviewScore,
                DistributionBasedOnRatingScore = reviewDistribution.Value,
                TotalReviewsNumber = totalReviewsNumber
            };

            return ReviewStatisticsDto;
        }
    }
}
