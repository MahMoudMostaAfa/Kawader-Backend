
using Kawadar.Domain.Portfolios.Project.Enum;

namespace Kawadar.Application.Features.Portfolios.DTOs
{
    public class ProjectDTO
    {
        public string title { get; set; }
        public string description { get; set; }
        public PortfolioProjectCategory category { get; set; }
        public int displayOrder { get; set; }
    }
}
