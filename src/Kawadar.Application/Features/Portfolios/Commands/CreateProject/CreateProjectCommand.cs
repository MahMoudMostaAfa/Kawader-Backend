using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Application.Features.Skills.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;


namespace Kawadar.Application.Features.Portfolios.Commands.CreateProject
{
    public record CreateProjectCommand(string Title, string Description,
        string specilization, IFormFile ProjectImage, string ProjectUrl, List<Guid> skills) : IRequest<Result<ProjectDTO>>;
}
