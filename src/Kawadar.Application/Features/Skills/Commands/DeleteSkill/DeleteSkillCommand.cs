using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Skills.Commands.DeleteSkill
{
    public record DeleteSkillCommand(Guid Id) : IRequest<Result<Deleted>>;
}
