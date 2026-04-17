
namespace Kawadar.Application.Features.Admins.Dtos
{
    public class UserStatisticsDto
    {
        public int usersCount { get; set; }
        public int FreelancersCount { get; set; }
        public int ClientsCount { get; set; }
        public int VerifiedUserCount { get; set; }
        public int NewUsersThisMonth { get; set; }
        public float averageUserRating { get; set; }
        public float LowestUserRating { get; set; }
        public float HighestUserRating { get; set; }
    }
}
