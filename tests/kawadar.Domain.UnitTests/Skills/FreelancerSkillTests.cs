
using Kawadar.Domain.Skills.FreelancerSkill;
using Kawadar.Domain.Skills.FreelancerSkill.Enum;
using Xunit;

namespace kawadar.Domain.UnitTests.Skills
{
    public class FreelancerSkillTests
    {
        [Fact]
        public void Create_WithNullCustomSkillName_ShouldSucceed()
        {
            var freelancerId = Guid.NewGuid();
            var skillId = Guid.NewGuid();

            var result = FreelancerSkill.Create(freelancerId, skillId, SkillType.Predefined, null);
            Assert.True(result.IsSuccess);
            var freelancerSkill = result.Value;
            Assert.Equal(freelancerId, freelancerSkill.FreelancerId);
            Assert.Equal(skillId, freelancerSkill.SkillId);
            Assert.Null(freelancerSkill.CustomSkillName);
        }

        [Fact]
        public void Create_WithNullSkillId_ShouldSucceed()
        {
            var freelancerId = Guid.NewGuid();
            var skillName = "skill";

            var result = FreelancerSkill.Create(freelancerId, null, SkillType.Predefined, skillName);
            Assert.True(result.IsSuccess);
            var freelancerSkill = result.Value;
            Assert.Equal(freelancerId, freelancerSkill.FreelancerId);
            Assert.Null(freelancerSkill.SkillId);
            Assert.Equal(skillName ,freelancerSkill.CustomSkillName);
        }

        [Fact]
        public void Create_WithEmptyFreelancerId_ShouldFail()
        {
            var freelancerId = Guid.Empty;
            var skillName = "skill";

            var result = FreelancerSkill.Create(freelancerId, null, SkillType.Predefined, skillName);
            Assert.True(result.IsError);
            Assert.Equal(FreelacnerSkillErrors.FreelancerIdIsRequired.Code, result.TopError.Code);
            Assert.Equal(FreelacnerSkillErrors.FreelancerIdIsRequired.Description, result.TopError.Description);
        }
    }
}
