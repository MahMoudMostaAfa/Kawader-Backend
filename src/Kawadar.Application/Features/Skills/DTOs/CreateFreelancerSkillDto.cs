using Kawadar.Domain.Skills.FreelancerSkill.Enum;

namespace Kawadar.Application.Features.Skills.DTOs
{
    public class CreateFreelancerSkillDto
    {
        public Guid? SkillId { get; set; }
        public SkillType SkillType { get; set; }
        public string? CustomSkillName { get; set; }
    }
}