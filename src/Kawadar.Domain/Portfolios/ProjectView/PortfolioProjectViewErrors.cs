using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Portfolios.ProjectView
{
    public class PortfolioProjectViewErrors
    {
        public static Error PortfolioProjectIdIsRequired => Error.Validation("PortfolioProjectView.PortfolioProjectIdIsRequired",
            "Portfolio Project Id is required");

        public static Error ViewedByIdIsRequired => Error.Validation("PortfolioProjectView.ViewedByIdIsRequired",
            "Viewed By Id is Required");
    }
}
