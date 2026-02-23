
using Azure.Core;
using Kawadar.Domain.Portfolios.ProjectView;
using Xunit;

namespace kawadar.Domain.UnitTests.Portfolio
{
    public class PortfolioProjectViewTests
    {
        [Fact]
        public void Create_ValidData_ShouldSucceed()
        {
            Guid userProfileId = Guid.NewGuid();
            Guid projectId = Guid.NewGuid();

            var result = PortfolioProjectView.Create(projectId, userProfileId);
            Assert.True(result.IsSuccess);

            var view = result.Value;
            Assert.Equal(userProfileId, view.UserProfileId);
            Assert.Equal(projectId, view.PortfolioProjectId);
        }

        [Fact]
        public void Create_EmptyUserProfileId_ShouldFail()
        {
            Guid userProfileId = Guid.Empty;
            Guid projectId = Guid.NewGuid();

            var result = PortfolioProjectView.Create(projectId, userProfileId);
            Assert.True(result.IsError);

            Assert.Equal(PortfolioProjectViewErrors.ViewedByIdIsRequired.Code, result.TopError.Code);
            Assert.Equal(PortfolioProjectViewErrors.ViewedByIdIsRequired.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_EmptyProjectId_ShouldFail()
        {
            Guid userProfileId = Guid.NewGuid();
            Guid projectId = Guid.Empty;

            var result = PortfolioProjectView.Create(projectId, userProfileId);
            Assert.True(result.IsError);

            Assert.Equal(PortfolioProjectViewErrors.PortfolioProjectIdIsRequired.Code, result.TopError.Code);
            Assert.Equal(PortfolioProjectViewErrors.PortfolioProjectIdIsRequired.Description, result.TopError.Description);
        }
    }
}
