using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Items.Enum;


namespace Kawadar.Domain.Portfolios.Items
{
    public class PortfolioItem: AuditableEntity
    {
        public ItemType itemType { get; private set; } = ItemType.Text;
        public string content { get; private set; } = "";
        public int displayOrder;

        //Foreign Key
        public Guid portfolioProjectId;

        private PortfolioItem(ItemType ItemType, string Content, int DisplayOrder, Guid PortfolioProjectId): base(Guid.NewGuid())
        {
            itemType = ItemType;
            content = Content;
            displayOrder = DisplayOrder;
            portfolioProjectId = PortfolioProjectId;
        }

        public static Result<PortfolioItem> Create(ItemType ItemType, string Content, int DisplayOrder, Guid PortfolioProjectId)
        {
            if(PortfolioProjectId == Guid.Empty)
            {
                return PortfolioItemErrors.PortfolioIdRequired;
            }

            if (string.IsNullOrWhiteSpace(Content))
            {
                return PortfolioItemErrors.ContentIsRequired;
            }

            var portfolioItem = new PortfolioItem(ItemType, Content, DisplayOrder, PortfolioProjectId);
            return portfolioItem;
        }

        public Result<Updated> Update(ItemType ItemType, string Content, int DisplayOrder)
        {
            itemType = ItemType;
            content = Content;
            displayOrder = DisplayOrder;

            UpdatedAt = DateTime.UtcNow;
            return Result.Updated;
        }
    }
}