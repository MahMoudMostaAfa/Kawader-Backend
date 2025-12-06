using Kawadar.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kawadar.Domain.Portfolios.ProjectView
{
    public interface IProjectViewRepository
    {
        public Task addAsync(PortfolioProjectView projectView);
        public Task<int> getViewsByProjectId(Guid projectId);
        public void Delete(Guid Id);
    }
}
