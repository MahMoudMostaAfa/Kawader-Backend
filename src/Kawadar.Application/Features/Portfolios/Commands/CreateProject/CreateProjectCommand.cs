using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Project.Enum;
using MediatR;


namespace Kawadar.Application.Features.Portfolios.Commands.CreateProject
{
    public record CreateProjectCommand(string Title, string Description, PortfolioProjectCategory Category) : IRequest<Result<Success>>;
}
