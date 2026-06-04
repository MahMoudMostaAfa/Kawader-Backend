
using Kawadar.Domain.UserProfiles.Enums;

namespace Kawadar.Application.Features.Admins.Dtos
{
    public class BriefUserProfileDto
    {
        public string fullName { get; set; } = "";
        public string UserName { get; set; } = "";
        public bool IsDeleted { get; set; }
        public bool IsOnline { get; set; }
        public string PhotoUrl { get; set; } = "";
        public float AverageRating { get; set; }
        public bool IsBanned { get; set; }
        public ProfileType profileType { get; set; }
    }
}
