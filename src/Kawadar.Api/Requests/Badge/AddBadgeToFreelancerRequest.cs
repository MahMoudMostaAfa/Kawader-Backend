namespace Kawadar.Api.Requests.Badge
{
    public class AddBadgeToFreelancerRequest
    {
        public Guid FreelancerId { get; set; }
        public Guid BadgeId { get; set; }
    }
}
