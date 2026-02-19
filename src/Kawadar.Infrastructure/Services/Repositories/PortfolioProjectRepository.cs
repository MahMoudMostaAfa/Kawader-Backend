using Kawadar.Application.Common.Interfaces.Repositories;
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
            if (Project == null) return Error.NotFound("Project.NotFound", "Project not found");

            return Project;
        }

        public async Task<Result<PortfolioItem>> GetProjectItemById(Guid ItemId)
        {
            var Item = await appDbContext.PortfolioItems.FirstOrDefaultAsync(i => i.Id == ItemId);
            if (Item == null) return Error.NotFound("Item.NotFound", "Item not found");
            return Item;
        }

        public async Task<Result<PortfolioProject>> GetWithItemsByProjectId(Guid PortfolioProjectId)
        {
            var Project = await appDbContext.PortfolioProjects.
                Include(p => p.Items).FirstOrDefaultAsync(p => p.Id == PortfolioProjectId);

            if (Project == null) return Error.NotFound("Project.NotFound", "Project not found");
            return Project;
        }

        public async Task<IEnumerable<PortfolioItem>> GetProjectItemsByProjectId(Guid projectId)
        {
            var items = await appDbContext.PortfolioItems.Where(x => x.PortfolioProjectId == projectId).ToListAsync();
            return items;
        }

        // add Skill


    }
}
