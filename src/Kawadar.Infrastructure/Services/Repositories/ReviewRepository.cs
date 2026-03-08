using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
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

        public async Task<Result<float>> GetAverageReviewScore(Guid UserProfileId)
        {
            var reviewScores = appDbContext.Reviews.Where(x =>x.RevieweeId == UserProfileId).Select(x => x.Rating);
            var average = await reviewScores.AverageAsync();
            return average;
        }

        public async Task<Result<Review>> GetReviewById(Guid Id)
        {
            var review = await appDbContext.Reviews.FirstOrDefaultAsync(x => x.Id == Id);
            if (review is null) return Error.NotFound("This review doesn't exist");
            return review;
        }

        public async Task<Result<PaginatedList<Review>>> GetReviewsByUserProfileId(float? Rating, int page, int pageSize, string sortBy, Guid Id)
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
    }
}
