using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Portfolios.Items
{
    public static class PortfolioItemErrors
    {
        public static Error PortfolioIdRequired => Error.Validation("PortfolioItem.PortfolioIdRequired",
            "Portfolio Id is Required to create an Portfolio Item");

        public static Error ContentIsRequired => Error.Validation("PortfolioItem.contentIsRequired",
            "Cann't create an item with no content");
    }
}
