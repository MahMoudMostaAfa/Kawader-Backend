using Kawadar.Application.Features.Portfolios.DTOs;

namespace Kawadar.Api.Requests.PortfolioProject
{
    public class ReorderProjectsRequest
    {
        public List<ProjectOrderDTO>? Order { get; set; }
    }
}
