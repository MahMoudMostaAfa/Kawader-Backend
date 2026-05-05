namespace Kawadar.Api.Requests.Subscriptions
{
    public class UpdateSubscriptionPlanRequest
    {
        public decimal price { get; set; }
        public int proposalsPerMonth { get; set; }
        public int PortfolioProjects { get; set; }
        public bool twentyFourSevenSupport { get; set; }
    }
}
