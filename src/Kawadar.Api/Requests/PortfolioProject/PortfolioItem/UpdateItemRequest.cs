using Kawadar.Domain.Portfolios.Items.Enum;

namespace Kawadar.Api.Requests.PortfolioProject.PortfolioItem
{
    public class UpdateItemRequest
    {
        public ItemType itemType { get; set; }
        public string? Content { get; set; }
        public IFormFile? Image { get; set; }
    }
}
