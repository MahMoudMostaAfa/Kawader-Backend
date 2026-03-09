
namespace Kawadar.Application.Features.Reviews.Dtos
{
    public class ReviewDto
    {
        public Guid JobId { get; set; }
        public string ReviewerUserName { get; set; } = "";
        public float Rating { get; set; }
        public string Comment { get; set; } = "";
    }
}
