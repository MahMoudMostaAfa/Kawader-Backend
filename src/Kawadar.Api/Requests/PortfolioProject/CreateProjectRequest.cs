namespace Kawadar.Api.Requests.PortfolioProject
{
    public class CreateProjectRequest
    {
        public string title { get; set; } = "";
        public string description { get; set; } = "";
        public IFormFile? ProjectImage { get; set; }
        public string? ProjectUrl { get; set; }
        public string specilizationName { get; set; } = "";
        public string skills { get; set; } = null!;
    }
}