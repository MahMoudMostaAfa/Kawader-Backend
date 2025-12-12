using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Items;
using Kawadar.Domain.Portfolios.Items.Enum;

namespace Kawadar.Domain.Portfolios.Project
{
    public interface IPortfolioProjectRepository
    {
        public Task<Result<Success>> AddAsync(PortfolioProject Project);
        public Task<Result<PortfolioProject>> GetPortfolioProjectById(Guid PortfolioProjectId);

        public Task<Result<PortfolioItem>> GetProjectItemById(Guid ItemId);
        public Task<IEnumerable<PortfolioProject>> GetAllByFreelancerId(Guid FreelancerId);
        public Task<Result<PortfolioProject>> GetWithItemsByProjectId(Guid PortfolioProjectId);
        public Result<Deleted> Delete(PortfolioProject Project);
        public Task<Result<Success>> AddItemAsync(PortfolioItem Item);
        public Result<Deleted> DeleteItem(PortfolioItem Item);

        // waiting for the skill entity implementation
        //public Task addSkill(Guid Skill);
    }
}