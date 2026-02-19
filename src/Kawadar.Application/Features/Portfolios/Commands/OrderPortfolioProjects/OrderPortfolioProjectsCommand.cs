using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.OrderPortfolioProjects
{
    public record OrderPortfolioProjectsCommand(List<ProjectOrderDTO> Order):IRequest<Result<Updated>>;
}