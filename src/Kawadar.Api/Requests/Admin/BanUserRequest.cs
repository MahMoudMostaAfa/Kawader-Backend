namespace Kawadar.Api.Requests.Admin
{
    public class BanUserRequest
    {
        public string UserName { get; set; } = "";
        public DateTime BannedUntil { get; set; }
    }
}
