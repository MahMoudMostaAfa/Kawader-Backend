using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Project;
using Kawadar.Domain.UserProfiles;

namespace Kawadar.Domain.Portfolios.ProjectView
{
    public class PortfolioProjectView: AuditableEntity
    {
        public Guid PortfolioProjectId { get; private set; }
        public Guid UserProfileId { get; private set; }

        public PortfolioProject PortfolioProject { get; private set; }

        public UserProfile UserProfile { get; private set; }

        private PortfolioProjectView(Guid portfolioProjectId, Guid UserProfileId) : base(Guid.NewGuid())
        {
            this.PortfolioProjectId = portfolioProjectId;
            this.UserProfileId = UserProfileId;
        }

        public static Result<PortfolioProjectView> Create(Guid portfolioProjectId, Guid viewedBy)
        {
            if(portfolioProjectId == Guid.Empty)
            {
                return PortfolioProjectViewErrors.PortfolioProjectIdIsRequired;
            }

            if(viewedBy == Guid.Empty)
            {
                return PortfolioProjectViewErrors.ViewedByIdIsRequired;
            }

            var view = new PortfolioProjectView(portfolioProjectId, viewedBy);
            return view;
        }
    }
}
