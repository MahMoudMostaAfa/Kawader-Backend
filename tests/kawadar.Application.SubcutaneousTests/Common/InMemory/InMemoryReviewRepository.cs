using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Reviews.Dtos;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Reviews;

namespace kawadar.Application.SubcutaneousTests.Common.InMemory;

public class InMemoryReviewRepository : IReviewRepository
{
    public readonly List<Review> Reviews = [];

    public Task AddReview(Review review, CancellationToken ct = default)
    {
        Reviews.Add(review);
        return Task.CompletedTask;
    }

    public Task<PaginatedList<Review>> GetReviewsByUserProfileId(float? Rating, int page, int pageSize, string sortBy, Guid Id)
    {
        var query = Reviews.Where(r => r.RevieweeId == Id);
        if (Rating.HasValue) query = query.Where(r => Math.Abs(r.Rating - Rating.Value) < 0.01f);

        // Apply sort before pagination
        var ordered = sortBy?.ToLowerInvariant() switch
        {
            "oldest"         => query.OrderBy(r => r.CreatedAt),
            "rating_asc"     => query.OrderBy(r => r.Rating),
            "rating_desc"    => query.OrderByDescending(r => r.Rating),
            _                => query.OrderByDescending(r => r.CreatedAt) // default: newest
        };

        var all = ordered.ToList();
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PaginatedList<Review>(items, all.Count, page, pageSize));
    }

    public Task<Result<ReviewStatisticsDto>> GetReviewsStatistics(Guid UserProfileId)
    {
        var userReviews = Reviews.Where(r => r.RevieweeId == UserProfileId).ToList();
        var stats = new ReviewStatisticsDto
        {
            AverageRating = userReviews.Count > 0 ? userReviews.Average(r => r.Rating) : 0,
            ReviewsCount = userReviews.Count
        };
        return Task.FromResult<Result<ReviewStatisticsDto>>(stats);
    }

    public Task<Result<Review>> GetReviewById(Guid Id)
    {
        var review = Reviews.FirstOrDefault(r => r.Id == Id);
        return Task.FromResult(review is not null
            ? (Result<Review>)review
            : Error.NotFound("Review.NotFound", $"Review '{Id}' not found."));
    }

    public Task<Result<Dictionary<float, int>>> GetReviewDistribution()
    {
        var dist = Reviews.GroupBy(r => r.Rating).ToDictionary(g => g.Key, g => g.Count());
        return Task.FromResult<Result<Dictionary<float, int>>>(dist);
    }

    public Task<Result<RatingStatisticsDto>> GetRatingStatistics()
    {
        var s = new RatingStatisticsDto();
        if (Reviews.Count > 0) { s.averageRating = Reviews.Average(r => r.Rating); s.HighestRated = Reviews.Max(r => r.Rating); s.LowestRated = Reviews.Min(r => r.Rating); }
        return Task.FromResult<Result<RatingStatisticsDto>>(s);
    }

    public Task<float> GetAverageReviewScore() => Task.FromResult(Reviews.Count > 0 ? Reviews.Average(r => r.Rating) : 0f);

    public void Clear() => Reviews.Clear();
}
