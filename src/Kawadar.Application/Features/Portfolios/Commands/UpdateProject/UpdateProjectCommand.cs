using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.UpdateProject
{
    public record UpdateProjectCommand(Guid Id, string ProjectUrl, string ImageUrl,
        int DisplayOrder, bool IsPublic) : IRequest<Result<Updated>>;
}
