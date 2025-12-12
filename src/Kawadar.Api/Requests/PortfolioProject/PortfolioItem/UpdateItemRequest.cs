using Kawadar.Domain.Portfolios.Items.Enum;

namespace Kawadar.Api.Requests.PortfolioProject.PortfolioItem
{
    public class UpdateItemRequest
    {
        public ItemType ItemType { get; set; }
        public string Content { get; set; }
        public int DisplayOrder { get; set; }
    }
}
