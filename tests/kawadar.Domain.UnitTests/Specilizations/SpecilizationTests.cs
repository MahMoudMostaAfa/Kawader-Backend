using Kawadar.Domain.Specilizations;
using Xunit;

namespace kawadar.Domain.UnitTests.Specilizations
{
    public class SpecilizationTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            string name = "special";
            var isActive = true;

            var result = Specilization.Create(name, isActive);
            Assert.True(result.IsSuccess);
            var specilization = result.Value;
            Assert.Equal(name, specilization.Name);
            Assert.Equal(isActive, specilization.IsActive);
        }

        [Fact]
        public void Create_WithEmptyName_ShouldFail()
        {
            string name = "";
            var isActive = true;

            var result = Specilization.Create(name, isActive);
            Assert.True(result.IsError);
            Assert.Equal(SpecilizationErros.NameIsRequired.Code, result.TopError.Code);
            Assert.Equal(SpecilizationErros.NameIsRequired.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithNullName_ShouldFail()
        {
            string? name = null;
            var isActive = true;

            var result = Specilization.Create(name, isActive);
            Assert.True(result.IsError);
            Assert.Equal(SpecilizationErros.NameIsRequired.Code, result.TopError.Code);
            Assert.Equal(SpecilizationErros.NameIsRequired.Description, result.TopError.Description);
        }

        [Fact]
        public void Update_WithValidData_ShouldSucceed()
        {
            string name = "special";
            var isActive = true;

            var result = Specilization.Create(name, isActive);
            Assert.True(result.IsSuccess);
            var specilization = result.Value;
            var newName = "Spec";
            var activeUpdate = false;
            var updateResult = specilization.Update(newName, activeUpdate);
            Assert.True(updateResult.IsSuccess);
            Assert.Equal(newName, specilization.Name);
            Assert.Equal(activeUpdate, specilization.IsActive);
        }
    }
}
