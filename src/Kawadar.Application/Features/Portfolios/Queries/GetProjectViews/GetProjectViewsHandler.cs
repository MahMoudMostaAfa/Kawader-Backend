using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.ProjectView;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Queries.GetProjectViews
{
    public class GetProjectViewsHandler(IUser user, IProjectViewRepository projectView)
        : IRequestHandler<GetProjectViewsQuery, Result<int>>
    {
        public async Task<Result<int>> Handle(GetProjectViewsQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await projectView.getViewsByProjectId(request.projectId);
            if (result.IsError) return result.Errors;

            return result.Value;
        }
    }
}