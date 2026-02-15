using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Portfolios.Project
{
    public class PortfolioProjectErrors
    {
        public static Error FreelancerIdIsRequired => Error.Validation("PortfolioProject.FreelancerIdIsRequired",
            "Freelacner Id is required to create a Portfolio Project");

        public static Error TitleIsRequired => Error.Validation("PortfolioProject.TitleIsRequired",
            "Title is required to create a portfolio project");

        public static Error DescriptionIsRequired => Error.Validation("PortfolioProject.DescriptionIsRequired",
            "Description is required to create a portfolio project");
    }
}
