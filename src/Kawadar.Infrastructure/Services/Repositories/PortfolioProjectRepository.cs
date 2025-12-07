
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Items;
using Kawadar.Domain.Portfolios.Project;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kawadar.Infrastructure.Services.Repositories
{
    internal class PortfolioProjectRepository(AppDbContext appDbContext) : IPortfolioProjectRepository
    {
        public async Task<Result<Success>> AddAsync(PortfolioProject Project)
        {
            await appDbContext.PortfolioProjects.AddAsync(Project);
            return Result.Success;
        }

        public async Task<Result<Success>> AddItemAsync(PortfolioItem Item)
        {
            await appDbContext.PortfolioItems.AddAsync(Item);
            return Result.Success;
        }

        public Result<Deleted> Delete(PortfolioProject Project)
        {
            appDbContext.PortfolioProjects.Remove(Project);
            return Result.Deleted;
        }

        public Result<Deleted> DeleteItem(PortfolioItem Item)
        {
            appDbContext.PortfolioItems.Remove(Item);
            return Result.Deleted;
        }

        public async Task<IEnumerable<PortfolioProject>> GetAllByFreelancerId(Guid FreelancerId)
        {
            var Projects = await appDbContext.PortfolioProjects.
                Where(s => s.FreelancerId == FreelancerId).ToListAsync();
            return Projects;
        }

        public async Task<Result<PortfolioProject>> GetPortfolioProjectById(Guid PortfolioProjectId)
        {
            var Project = await appDbContext.PortfolioProjects.FirstOrDefaultAsync(s => s.Id == PortfolioProjectId);
            return Project;
        }

        public async Task<Result<int>> GetProjectViews(Guid ProjectId)
        {
            var Views = await appDbContext.ProjectViews.Where(v => v.PortfolioProjectId == ProjectId).CountAsync();
            return Views;
        }

        public async Task<Result<PortfolioProject>> GetWithItemsByProjectId(Guid PortfolioProjectId)
        {
            var Project = await appDbContext.PortfolioProjects.
                Include(p => p.Items).FirstOrDefaultAsync(p => p.Id == PortfolioProjectId);
            return Project;
        }

        // add Skill

        
    }
}
