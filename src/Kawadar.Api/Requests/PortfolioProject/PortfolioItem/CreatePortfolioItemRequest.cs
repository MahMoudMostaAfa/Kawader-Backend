using Kawadar.Domain.Portfolios.Items.Enum;

namespace Kawadar.Api.Requests.PortfolioProject.PortfolioItem
{
    public class CreatePortfolioItemRequest
    {
        public ItemType ItemType { get; set; } 
        public string Content { get; set; } 
    }
}
