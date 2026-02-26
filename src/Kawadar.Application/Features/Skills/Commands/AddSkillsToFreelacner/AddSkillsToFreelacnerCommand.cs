using Kawadar.Application.Features.Skills.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Skills.FreelancerSkill;
using MediatR;

namespace Kawadar.Application.Features.Skills.Commands.AddSkillsToFreelacner
{
    public record AddSkillsToFreelacnerCommand(List<CreateFreelancerSkillDto> Skills) : IRequest<Result<List<FreelancerSkill>>>;
}
