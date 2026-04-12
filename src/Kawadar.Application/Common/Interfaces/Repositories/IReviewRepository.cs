using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Reviews.Dtos;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Reviews;

namespace Kawadar.Application.Common.Interfaces.Repositories
{
    public interface IReviewRepository
    {
        public Task AddReview(Review review, CancellationToken ct = default);
        public Task<PaginatedList<Review>> GetReviewsByUserProfileId(float? Rating, int page, int pageSize, string sortBy, Guid Id);
        public Task<Result<ReviewStatisticsDto>> GetReviewsStatistics(Guid UserProfileId);
        public Task<Result<Review>> GetReviewById(Guid Id);
        public Task<Result<Dictionary<float, int>>> GetReviewDistribution();
        public Task<Result<RatingStatisticsDto>> GetRatingStatistics();

    }
}