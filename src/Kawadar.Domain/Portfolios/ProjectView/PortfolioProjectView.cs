using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Portfolios.ProjectView
{
    public class PortfolioProjectView: AuditableEntity
    {
        public Guid PortfolioProjectId { get; private set; }
        public Guid ViewedBy { get; private set; }

        private PortfolioProjectView(Guid portfolioProjectId, Guid viewedBy): base(Guid.NewGuid())
        {
            PortfolioProjectId = portfolioProjectId;
            ViewedBy = viewedBy;
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
