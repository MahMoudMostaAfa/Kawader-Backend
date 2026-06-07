using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Reviews.Enums;

namespace Kawadar.Domain.Reviews
{
    public class Review : AuditableEntity
    {
        public Guid JobId { get; private set; }
        public Guid ReviewerId { get; private set; }
        public Guid RevieweeId { get; private set; }
        public ReviewType ReviewType { get; private set; }
        public float Rating { get; private set; }
        public string Comment { get; private set; } = "";

        private Review(Guid jobId, Guid reviewerId, Guid revieweeId, ReviewType reviewType, float rating, string comment)
        {
            JobId = jobId;
            ReviewerId = reviewerId;
            RevieweeId = revieweeId;
            ReviewType = reviewType;
            Rating = rating;
            Comment = comment;
        }

        public static Result<Review> Create(Guid jobId, Guid reviewerId, Guid revieweeId, ReviewType reviewType, float rating, string comment)
        {
            return new Review(jobId, reviewerId, revieweeId, reviewType, rating, comment);
        }

        public Result<Updated> Update(float rating, string comment)
        {
            Rating = rating;
            Comment = comment;

            UpdatedAt = DateTime.UtcNow;
            return Result.Updated;
        }
    }
}
