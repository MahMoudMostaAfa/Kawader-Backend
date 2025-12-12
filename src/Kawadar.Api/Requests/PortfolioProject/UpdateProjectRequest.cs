using System.Reflection;

namespace Kawadar.Api.Requests.PortfolioProject
{
    public class UpdateProjectRequest
    {
        public string ProjectUrl { get; set; }
        public string ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool isPublic { get; set; }
    }
}
