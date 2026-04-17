
namespace Kawadar.Application.Features.Admins.Dtos
{
    public class ReviewStatisticsDto
    {
        public int TotalReviewsNumber { get; set; }
        public float AverageReviewScore { get; set; }
        public Dictionary<float, int>? DistributionBasedOnRatingScore { get; set; }
    }
}
