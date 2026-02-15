
using Kawadar.Domain.Portfolios.Project.Enum;

namespace Kawadar.Application.Features.Portfolios.DTOs
{
    public class ProjectDTO
    {
        public Guid Id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public PortfolioProjectCategory category { get; set; }
        public string ProjectImageUrl { get; set; }
        public string ProjectUrl { get; set; }
        public int displayOrder { get; set; }
    }
}
