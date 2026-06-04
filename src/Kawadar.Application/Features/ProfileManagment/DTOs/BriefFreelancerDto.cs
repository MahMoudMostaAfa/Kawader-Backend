
namespace Kawadar.Application.Features.ProfileManagment.DTOs
{
    public class BriefFreelancerDto
    {
        public Guid Id { get; set; }
        public string fullName { get; set; } = "";
        public string UserName { get; set; } = "";
        public bool IsOnline { get; set; }
        public string PhotoUrl { get; set; } = "";
        public float AverageRating { get; set; }
    }
}
