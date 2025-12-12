using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Items.Enum;
using Kawadar.Domain.Portfolios.Project;


namespace Kawadar.Domain.Portfolios.Items
{
    public class PortfolioItem: AuditableEntity
    {
        public ItemType ItemType { get; private set; } = ItemType.Text;
        public string Content { get; private set; } = "";
        public int DisplayOrder { get; private set; }

        //Foreign Key
        public Guid PortfolioProjectId { get; private set; }

        public PortfolioProject PortfolioProject { get; private set; }

        private PortfolioItem(ItemType ItemType, string Content, int DisplayOrder, Guid PortfolioProjectId): base(Guid.NewGuid())
        {
            this.ItemType = ItemType;
            this.Content = Content;
            this.DisplayOrder = DisplayOrder;
            this.PortfolioProjectId = PortfolioProjectId;
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
            this.ItemType = ItemType;
            this.Content = Content;
            this.DisplayOrder = DisplayOrder;

            UpdatedAt = DateTime.UtcNow;
            return Result.Updated;
        }
    }
}