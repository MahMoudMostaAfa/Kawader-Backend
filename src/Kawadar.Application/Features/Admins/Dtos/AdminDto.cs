
namespace Kawadar.Application.Features.Admins.Dtos
{
    public class AdminDto
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public bool IsDeleted { get; set; } = false;
        public bool IsOnline { get; set; } = false;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
