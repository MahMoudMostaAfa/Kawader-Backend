using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Project.Enum;
using MediatR;
using Microsoft.AspNetCore.Http;


namespace Kawadar.Application.Features.Portfolios.Commands.CreateProject
{
    public record CreateProjectCommand(string Title, string Description,
        PortfolioProjectCategory Category, IFormFile ProjectImage, string ProjectUrl) : IRequest<Result<ProjectDTO>>;
}
