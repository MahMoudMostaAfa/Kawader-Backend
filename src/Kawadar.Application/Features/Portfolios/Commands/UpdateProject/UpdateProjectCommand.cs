using Kawadar.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Kawadar.Application.Features.Portfolios.Commands.UpdateProject
{
    public record UpdateProjectCommand(Guid Id, string ProjectUrl, IFormFile Image,
        int DisplayOrder, bool IsPublic) : IRequest<Result<Updated>>;
}
