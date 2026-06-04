namespace Kawadar.Application.Features.ProfileManagment.DTOs
{
    public class BriefFreelancerWithStrengthDto
    {
        public Guid Id { get; set; }
        public string fullName { get; set; } = "";
        public string UserName { get; set; } = "";
        public bool IsOnline { get; set; }
        public List<string> Strength { get; set; } = default!;
        public string PhotoUrl { get; set; } = "";
        public float AverageRating { get; set; }
    }
}
