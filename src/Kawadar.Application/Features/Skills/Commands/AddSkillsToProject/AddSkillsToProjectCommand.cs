using Kawadar.Application.Features.Skills.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.ProjectSkill;
using MediatR;

namespace Kawadar.Application.Features.Skills.Commands.AddSkillsToProject
{
    public record AddSkillsToProjectCommand(Guid ProjectId, List<CreateProjectSkillDto> skills) : IRequest<Result<List<PortfolioProjectSkill>>>;
}
