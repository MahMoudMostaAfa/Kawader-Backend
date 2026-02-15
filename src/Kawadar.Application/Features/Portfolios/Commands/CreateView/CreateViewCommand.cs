using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.ProjectView;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.CreateView
{
    public record CreateViewCommand(Guid ProjectId) : IRequest<Result<PortfolioProjectView>>;
}
