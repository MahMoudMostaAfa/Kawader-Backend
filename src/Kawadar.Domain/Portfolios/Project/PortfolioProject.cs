using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Items;

namespace Kawadar.Domain.Portfolios.Project
{
    public class PortfolioProject : AuditableEntity
    {
        public string Title { get; private set; } = "";
        public string Description { get; private set; } = "";
        public Guid SpecilizationId { get; private set; }
        public DateTime? ProjectDate { get; private set; } = DateTime.UtcNow;
        public string? ProjectUrl { get; private set; } = "";
        public string ? ProjectImageUrl { get; private set; } = "";
        public int DisplayOrder { get; private set; } = 1;
        public bool IsPublic { get; private set; } = true;
        private readonly List<PortfolioItem> _items = new();
        public IReadOnlyCollection<PortfolioItem> Items => _items.AsReadOnly();

        public Guid FreelancerId { get; private set; }


        private PortfolioProject(string Title, string Description, Guid SpecilizationId,
             Guid FreelancerId): base(Guid.NewGuid())
        {
            this.Title = Title;
            this.Description = Description;
            this.SpecilizationId = SpecilizationId;
            this.FreelancerId = FreelancerId;
        }

        public static Result<PortfolioProject> Create(string Title, string Description, Guid specilization,
             Guid FreelancerId, string ProjectImageUrl, int displayOrder, string ProjectUrl = "")
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
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

            if(specilization == Guid.Empty)
            {
                return PortfolioProjectErrors.SpecilizaitonIsRequired;
            }

            var Project = new PortfolioProject(
                Title,
                Description,
                specilization,
                FreelancerId
                );

            Project.ProjectImageUrl = ProjectImageUrl;
            Project.ProjectUrl = ProjectUrl;
            Project.DisplayOrder = displayOrder;

            return Project;
        }

        public Result<Updated> Update(string ProjectUrl, string ImageUrl, bool IsPublic)
        {
            this.ProjectUrl = ProjectUrl;
            ProjectImageUrl = ImageUrl;
            this.DisplayOrder = DisplayOrder;
            this.IsPublic = IsPublic;

            UpdatedAt = DateTime.Now;
            return Result.Updated;
        }

        public Result<Updated> UpdateOrder(int DisplayOrder)
        {
            this.DisplayOrder = DisplayOrder;

            UpdatedAt = DateTime.Now;
            return Result.Updated;
        }
    }
}