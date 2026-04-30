using Kawadar.Domain.Skills;
using Xunit;

namespace kawadar.Domain.UnitTests.Skills
{
    public class SkillTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var name = "skill";
            Guid CreatedBy = Guid.NewGuid();
            var isActive = true;

            var result = Skill.Create(name, isActive, CreatedBy);
            Assert.True(result.IsSuccess);
            var skill = result.Value;
            Assert.Equal(name, skill.Name);
            Assert.Equal(CreatedBy, skill.CreatedBy);
            Assert.Equal(isActive, skill.IsActive);
        }

        [Fact]
        public void Create_WithEmptyName_ShouldSucceed()
        {
            var name = "";
            Guid CreatedBy = Guid.NewGuid();
            var isActive = true;

            var result = Skill.Create(name, isActive, CreatedBy);
            Assert.True(result.IsError);
            Assert.Equal(SkillErrors.NameIsRequired.Code, result.TopError.Code);
            Assert.Equal(SkillErrors.NameIsRequired.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithNullName_ShouldSucceed()
        {
            string? name = null;
            Guid CreatedBy = Guid.NewGuid();
            var isActive = true;

            var result = Skill.Create(name, isActive, CreatedBy);
            Assert.True(result.IsError);
            Assert.Equal(SkillErrors.NameIsRequired.Code, result.TopError.Code);
            Assert.Equal(SkillErrors.NameIsRequired.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithEmptyCreatedBy_ShouldSucceed()
        {
            var name = "skill";
            Guid CreatedBy = Guid.Empty;
            var isActive = true;

            var result = Skill.Create(name, isActive, CreatedBy);
            Assert.True(result.IsError);
            Assert.Equal(SkillErrors.CreatorIdIsRequired.Code, result.TopError.Code);
            Assert.Equal(SkillErrors.CreatorIdIsRequired.Description, result.TopError.Description);
        }

        [Fact]
        public void Update_WithValidData_ShouldSucceed()
        {
            var name = "skill";
            Guid CreatedBy = Guid.NewGuid();
            var isActive = true;

            var result = Skill.Create(name, isActive, CreatedBy);
            Assert.True(result.IsSuccess);
            var skill = result.Value;
            var updateName = "New skill";
            bool activeUpdate = false;
            var updateResult = skill.Update(updateName, activeUpdate);
            Assert.True(updateResult.IsSuccess);
            Assert.Equal(updateName, skill.Name);
            Assert.Equal(activeUpdate, skill.IsActive);
        }
    }
}
