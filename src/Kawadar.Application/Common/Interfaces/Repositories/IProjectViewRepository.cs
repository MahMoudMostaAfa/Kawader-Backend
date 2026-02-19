using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.ProjectView;

namespace Kawadar.Application.Common.Interfaces.Repositories
{
    public interface IProjectViewRepository
    {
        public Task<Result<Success>> addAsync(PortfolioProjectView projectView);
        public Task<Result<int>> getViewsByProjectId(Guid projectId);
    }
}
