using Kawadar.Domain.Portfolios.Project.Enum;

namespace Kawadar.Api.Requests.PortfolioProject
{
    public class CreateProjectRequest
    {
        public string title { get; set; }
        public string description { get; set; }

        public IFormFile? ProjectImage { get; set; }
        public string? ProjectUrl { get; set; }
        public PortfolioProjectCategory category { get; set; }
    }
}