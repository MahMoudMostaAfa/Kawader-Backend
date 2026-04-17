using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Reviews.Dtos;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Reviews;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kawadar.Infrastructure.Services.Repositories
{
    public class ReviewRepository(AppDbContext appDbContext) : IReviewRepository
    {
        public async Task AddReview(Review review, CancellationToken ct = default)
        {
            await appDbContext.Reviews.AddAsync(review, ct);
        }

        public async Task<Result<ReviewStatisticsDto>> GetReviewsStatistics(Guid UserProfileId)
        {
            var count = await appDbContext.Reviews.Where(x => x.RevieweeId == UserProfileId).CountAsync();
            float average = 0;
            var ratings = appDbContext.Reviews.Where(x => x.RevieweeId == UserProfileId).Select(x => x.Rating);
            if (ratings.Count() != 0) average = await ratings.AverageAsync();
            return new ReviewStatisticsDto
            {
                AverageRating = average,
                ReviewsCount = count
            };
        }

        public async Task<Result<Review>> GetReviewById(Guid Id)
        {
            var review = await appDbContext.Reviews.FirstOrDefaultAsync(x => x.Id == Id);
            if (review is null) return Error.NotFound("This review doesn't exist");
            return review;
        }

        public async Task<PaginatedList<Review>> GetReviewsByUserProfileId(float? Rating, int page, int pageSize, string sortBy, Guid Id)
        {
            var query = appDbContext.Reviews.AsQueryable();
            if (Rating.HasValue)
            {
                query = query.Where(x => x.Rating == Rating);
            }

            if(sortBy == "oldest")
            {
                query.OrderBy(x => x.CreatedAt);
            }
            else if(sortBy == "newest")
            {
                query.OrderByDescending(x => x.CreatedAt);
            }
            else if(sortBy == "highest")
            {
                query.OrderByDescending(x => x.Rating);
            }
            else
            {
                query.OrderBy(x => x.Rating);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                  .Skip((page - 1) * pageSize)
                  .Take(pageSize)
                  .ToListAsync();

            return new PaginatedList<Review>(items, totalCount, page, pageSize);
        }

        public async Task<Result<Dictionary<float, int>>> GetReviewDistribution()
        {
            var distribution = await appDbContext.Reviews.GroupBy(x => x.Rating).ToDictionaryAsync(x => x.Key, x => x.Count());
            return distribution;
        }

        public async Task<float> GetAverageReviewScore()
        {
            var reviewsCount = await appDbContext.Reviews.CountAsync();
            float averageRating = 0;
            if (reviewsCount > 0) averageRating = await appDbContext.Reviews.Select(x => x.Rating).AverageAsync();
            return averageRating;
        }

        public async Task<Result<RatingStatisticsDto>> GetRatingStatistics()
        {
            var reviewsCount = await appDbContext.Reviews.CountAsync();
            float averageRating = 0;
            if(reviewsCount > 0) averageRating = await appDbContext.Reviews.Select(x => x.Rating).AverageAsync();
            var UserReviews = appDbContext.Reviews.GroupBy(x => x.RevieweeId).Select(x => x.Average(x => x.Rating));
            float highestRating = 0;
            float lowestRating = 0;
            if(UserReviews.Count() > 0)
            {
                highestRating = await UserReviews.MaxAsync();
                lowestRating = await UserReviews.MinAsync();
            }
            return new RatingStatisticsDto
            {
                averageRating = averageRating,
                HighestRated = highestRating,
                LowestRated = lowestRating
            };
            
        }
    }
}