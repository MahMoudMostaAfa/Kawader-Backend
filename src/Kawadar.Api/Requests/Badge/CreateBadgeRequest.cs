namespace Kawadar.Api.Requests.Badge
{
    public class CreateBadgeRequest
    {
        public string title { get; set; } = "";
        public string description { get; set; } = "";
        public IFormFile? Icon { get; set; }
    }
}
