using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Skills;
using MediatR;

namespace Kawadar.Application.Features.Skills.Queries.GetSkillById
{
    public record GetSkillByIdQuery(Guid Id) : IRequest<Result<Skill>>;
}
