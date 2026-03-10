using Kawadar.Application.Features.Skills.DTOs;

namespace Kawadar.Api.Requests.Skill
{
    public class AddSkillsToProjectRequest
    {
        public Guid projectId { get; set; }
        public List<CreateProjectSkillDto>? skills { get; set; }
    }
}
