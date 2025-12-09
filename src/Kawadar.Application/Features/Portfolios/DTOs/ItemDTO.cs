using Kawadar.Domain.Portfolios.Items.Enum;

namespace Kawadar.Application.Features.Portfolios.DTOs
{
    public class ItemDTO
    {
        public ItemType itemType { get; set; }
        public string content { get; set; }
        public int displayOrder { get; set; }
    }
}
