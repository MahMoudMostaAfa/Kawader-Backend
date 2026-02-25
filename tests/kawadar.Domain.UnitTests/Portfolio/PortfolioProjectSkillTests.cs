
using Kawadar.Domain.Portfolios.ProjectSkill;
using Xunit;

namespace kawadar.Domain.UnitTests.Portfolio
{
    public class PortfolioProjectSkillTests
    {
        [Fact]
        public void Create_ValidData_ShouldSucceed()
        {
            Guid ProjectId = Guid.NewGuid();
            Guid skillId = Guid.NewGuid();

            var result = PortfolioProjectSkill.Create(ProjectId, skillId);
            Assert.True(result.IsSuccess);

            var projectSkill = result.Value;
            Assert.Equal(ProjectId, projectSkill.PortfolioProjectId);
            Assert.Equal(skillId, projectSkill.SkillId);
        }

        [Fact]
        public void Create_EmptyProjectId_ShouldFail()
        {
            Guid ProjectId = Guid.Empty;
            Guid skillId = Guid.NewGuid();

            var result = PortfolioProjectSkill.Create(ProjectId, skillId);
            Assert.True(result.IsError);
            Assert.Equal(ProjectSkillErrors.PortfolioProjectIdIsRequired.Code, result.TopError.Code);
            Assert.Equal(ProjectSkillErrors.PortfolioProjectIdIsRequired.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_EmptySkillId_ShouldFail()
        {
            Guid ProjectId = Guid.NewGuid();
            Guid skillId = Guid.Empty;

            var result = PortfolioProjectSkill.Create(ProjectId, skillId);
            Assert.True(result.IsError);
            Assert.Equal(ProjectSkillErrors.SkillIdIsRequired.Code, result.TopError.Code);
            Assert.Equal(ProjectSkillErrors.SkillIdIsRequired.Description, result.TopError.Description);
        }
    }
}
