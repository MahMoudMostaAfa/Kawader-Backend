using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Portfolios.ProjectView
{
    public interface IProjectViewRepository
    {
        public Task<Result<Success>> addAsync(PortfolioProjectView projectView);
        public Task<Result<int>> getViewsByProjectId(Guid projectId);
    }
}
