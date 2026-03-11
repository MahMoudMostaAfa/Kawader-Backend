using System.Reflection;

namespace Kawadar.Api.Requests.PortfolioProject
{
    public class UpdateProjectRequest
    {
        public string ProjectUrl { get; set; } = "";
        public IFormFile? Image { get; set; }
        public bool isPublic { get; set; }
    }
}
