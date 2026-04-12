using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Skills.Commands.RemoveSkillFromFreelancer
{
    public record RemoveSkillFromFreelancerCommand(string skillName) : IRequest<Result<Deleted>>;
}
