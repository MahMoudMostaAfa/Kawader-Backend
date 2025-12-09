using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Portfolios.Items;

namespace Kawadar.Application.Features.Portfolios.Mapper
{
    public static class ItemMapper
    {
        public static ItemDTO toDTO(this PortfolioItem item)
        {
            var DTO = new ItemDTO { content = item.content, displayOrder = item.displayOrder, itemType = item.itemType };
            return DTO;
        }

        public static List<ItemDTO> toDTOList(this IEnumerable<PortfolioItem> items)
        {
            return items.Select(i => i.toDTO()).ToList();
        }
    }
}
