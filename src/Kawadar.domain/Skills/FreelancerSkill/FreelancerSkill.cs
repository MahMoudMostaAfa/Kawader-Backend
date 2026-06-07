using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Skills.FreelancerSkill.Enum;

namespace Kawadar.Domain.Skills.FreelancerSkill
{
    public class FreelancerSkill : AuditableEntity
    {
        public Guid FreelancerId { get; private set; }
        public Guid? SkillId { get; private set; }
        public SkillType SkillType { get; private set; }
        public string? CustomSkillName { get; private set; }

        private FreelancerSkill(Guid freelancerId, Guid? skillId, SkillType skillType, string? customSkillName)
        {
            FreelancerId = freelancerId;
            SkillId = skillId;
            SkillType = skillType;
            CustomSkillName = customSkillName;
        }

        public static Result<FreelancerSkill> Create(Guid freelancerId, Guid? skillId, SkillType skillType, string? customSkillName)
        {
            if(freelancerId == Guid.Empty)
            {
                return FreelacnerSkillErrors.FreelancerIdIsRequired;
            }

            var FreelancerSkill = new FreelancerSkill(freelancerId, skillId, skillType, customSkillName);
            return FreelancerSkill;
        }
    }
}
