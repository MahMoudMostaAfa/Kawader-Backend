using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Items;
using Kawadar.Domain.Portfolios.Project.Enum;

namespace Kawadar.Domain.Portfolios.Project
{
    public class PortfolioProject: AuditableEntity
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public PortfolioProjectCategory Category { get; private set; }
        public DateTime? ProjectDate { get; private set; }
        public string? ProjectUrl { get; private set; }
        public string ProjectImageUrl { get; private set; } = "";
        public int DisplayOrder { get; private set; }
        public bool IsPublic { get; private set; } = true;
        private readonly List<PortfolioItem> _items = new();
        public IReadOnlyCollection<PortfolioItem> Items => _items.AsReadOnly();

        public Guid FreelancerId { get; private set; }


        private PortfolioProject(string Title, string Description, PortfolioProjectCategory Category, DateTime Date,
            string ProjectUrl, string ImageUrl, int DisplayOrder, bool IsPublic, Guid FreelancerId): base(Guid.NewGuid())
        {
            this.Title = Title;
            this.Description = Description;
            this.Category = Category;
            ProjectDate = Date;
            this.ProjectUrl = ProjectUrl;
            ProjectImageUrl = ImageUrl;
            this.DisplayOrder = DisplayOrder;
            this.IsPublic = IsPublic;
            this.FreelancerId = FreelancerId;
        }

        public static Result<PortfolioProject> Create(string Title, string Description, PortfolioProjectCategory Category, DateTime Date,
            string ProjectUrl, string ImageUrl, int DisplayOrder, bool IsPublic, Guid FreelancerId)
        {
            if (string.IsNullOrWhiteSpace(Title)){
                return PortfolioProjectErrors.TitleIsRequired;
            }

            if (string.IsNullOrWhiteSpace(Description))
            {
                return PortfolioProjectErrors.DescriptionIsRequired;
            }

            if (FreelancerId == Guid.Empty)
            {
                return PortfolioProjectErrors.FreelancerIdIsRequired;
            }

            var Project = new PortfolioProject(
                Title,
                Description,
                Category,
                Date,
                ProjectUrl,
                ImageUrl,
                DisplayOrder,
                IsPublic,
                FreelancerId);

            return Project;
        }

        public Result<Updated> Update(string ProjectUrl, string ImageUrl, int DisplayOrder, bool IsPublic)
        {
            this.ProjectUrl = ProjectUrl;
            ProjectImageUrl = ImageUrl;
            this.DisplayOrder = DisplayOrder;
            this.IsPublic = IsPublic;

            UpdatedAt = DateTime.Now;
            return Result.Updated;
        }
    }
}