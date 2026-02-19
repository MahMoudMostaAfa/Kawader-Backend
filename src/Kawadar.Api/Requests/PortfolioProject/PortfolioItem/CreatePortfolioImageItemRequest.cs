using Kawadar.Domain.Portfolios.Items.Enum;

namespace Kawadar.Api.Requests.PortfolioProject.PortfolioItem
{
    public class CreatePortfolioImageItemRequest
    {
        public ItemType ItemType { get; set; }
        public IFormFile Image { get; set; }
    }
}
