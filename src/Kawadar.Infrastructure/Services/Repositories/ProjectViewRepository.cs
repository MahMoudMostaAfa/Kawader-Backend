using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.ProjectView;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kawadar.Infrastructure.Services.Repositories
{
    public class ProjectViewRepository(AppDbContext appDbContext) : IProjectViewRepository
    {
        public async Task<Result<Success>> addAsync(PortfolioProjectView projectView)
        {
            await appDbContext.ProjectViews.AddAsync(projectView);
            return Result.Success;
        }


        public async Task<Result<int>> getViewsByProjectId(Guid projectId)
        {
            var Views = await appDbContext.ProjectViews.Where(v => v.PortfolioProjectId == projectId).CountAsync();
            return Views;
        }
    }
}