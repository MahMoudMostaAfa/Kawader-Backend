using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Queries.GetProjectViews
{
    public record GetProjectViewsQuery(Guid projectId) : IRequest<Result<int>>;
}
