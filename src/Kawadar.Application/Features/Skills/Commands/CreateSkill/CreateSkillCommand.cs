using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Skills;
using MediatR;

namespace Kawadar.Application.Features.Skills.Commands.CreateSkill
{
    public record CreateSkillCommand(string name, bool isActive) : IRequest<Result<Skill>>;
}
