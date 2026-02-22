
namespace Kawadar.Application.Features.Badges.DTOs
{
    public class BadgeDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string IconUrl { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
