using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Reviews;

namespace Kawadar.Application.Common.Interfaces.Repositories
{
    public interface IReviewRepository
    {
        public Task AddReview(Review review, CancellationToken ct = default);
        public Task<Result<PaginatedList<Review>>> GetReviewsByUserProfileId(float? Rating, int page, int pageSize, string sortBy, Guid Id);
        public Task<Result<float>> GetAverageReviewScore(Guid UserProfileId);
        public Task<Result<Review>> GetReviewById(Guid Id);
    }
}