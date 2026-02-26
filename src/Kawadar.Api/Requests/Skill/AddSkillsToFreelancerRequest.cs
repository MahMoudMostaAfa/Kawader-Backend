using Kawadar.Application.Features.Skills.DTOs;

namespace Kawadar.Api.Requests.Skill
{
    public class AddSkillsToFreelancerRequest
    {
        public List<CreateFreelancerSkillDto> skills { get; set; } = new();
    }
}
