using Kawadar.Domain.Portfolios.Items.Enum;

namespace Kawadar.Api.Requests.PortfolioProject.PortfolioItem
{
    public class UpdatePortfolioImageRequest
    {
        public IFormFile Image { get; set; }
    }
}
