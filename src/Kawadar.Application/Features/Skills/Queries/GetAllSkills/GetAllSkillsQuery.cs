using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Skills;
using MediatR;

namespace Kawadar.Application.Features.Skills.Queries.GetAllSkills
{
    public record GetAllSkillsQuery() : IRequest<Result<List<Skill>>>;
}
