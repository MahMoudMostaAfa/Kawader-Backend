using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Items;

namespace Kawadar.Domain.Portfolios.Project
{
    public interface IPortfolioProjectRepository
    {
        public Task addAsync(PortfolioProject Project);
        public Task<Result<PortfolioProject>> getPortfolioProjectById(Guid PortfolioProjectId);
        public Task<IEnumerable<PortfolioProject>> getAllByFreelancerId(Guid FreelancerId);
        public Task<Result<PortfolioProject>> getWithItemsByProjectId(Guid PortfolioProjectId);
        public void Delete(PortfolioProject Project);
        public Task addItemAsync(PortfolioItem Item);
        public void deleteItem(PortfolioItem Item);
        
    }
}