
using Kawadar.Domain.Portfolios.Project;
using Kawadar.Domain.Portfolios.Project.Enum;
using Xunit;

namespace kawadar.Domain.UnitTests.Portfolio
{
    public class PortfolioProjectTests
    {
        [Fact]
        public void Create_WithValidDataAndNoProjectUrl_ShouldSucceed()
        {
            string title = "Kawader";
            string description = "A freelancing platform";
            string ImageUrl = "www.image.com";
            int displayOrder = 1;
            Guid freelancerId = Guid.NewGuid();
            var projectResult = PortfolioProject.Create(title, description, PortfolioProjectCategory.BackendDevelopment, freelancerId, ImageUrl, displayOrder);

            Assert.True(projectResult.IsSuccess);
            var project = projectResult.Value;
            Assert.Equal(title, project.Title);
            Assert.Equal(description, project.Description);
            Assert.Equal(ImageUrl, project.ProjectImageUrl);
            Assert.Equal(displayOrder, project.DisplayOrder);
            Assert.Equal(freelancerId, project.FreelancerId);
            Assert.Equal(string.Empty, project.ProjectUrl);
        }

        [Fact]
        public void Create_WithValidDataAndProjectUrl_ShouldSucceed()
        {
            string title = "Kawader";
            string description = "A freelancing platform";
            string ImageUrl = "www.image.com";
            int displayOrder = 1;
            Guid freelancerId = Guid.NewGuid();
            string projectUrl = "www.Project.com";
            var projectResult = PortfolioProject.Create(title, description, PortfolioProjectCategory.BackendDevelopment, freelancerId, ImageUrl, displayOrder, projectUrl);

            Assert.True(projectResult.IsSuccess);
            var project = projectResult.Value;
            Assert.Equal(title, project.Title);
            Assert.Equal(description, project.Description);
            Assert.Equal(ImageUrl, project.ProjectImageUrl);
            Assert.Equal(displayOrder, project.DisplayOrder);
            Assert.Equal(freelancerId, project.FreelancerId);
            Assert.Equal(projectUrl, project.ProjectUrl);
        }


        [Fact]
        public void Create_WithEmptyTitle_ShouldFail()
        {
            string title = "";
            string description = "A freelancing platform";
            string ImageUrl = "www.image.com";
            int displayOrder = 1;
            Guid freelancerId = Guid.NewGuid();
            string projectUrl = "www.Project.com";
            var projectResult = PortfolioProject.Create(title, description, PortfolioProjectCategory.BackendDevelopment, freelancerId, ImageUrl, displayOrder, projectUrl);

            Assert.True(projectResult.IsError);
            Assert.Equal(PortfolioProjectErrors.TitleIsRequired.Code, projectResult.TopError.Code);
            Assert.Equal(PortfolioProjectErrors.TitleIsRequired.Description, projectResult.TopError.Description);
        }

        [Fact]
        public void Create_WithEmptyDescription_ShouldFail()
        {
            string title = "Kawader";
            string description = "";
            string ImageUrl = "www.image.com";
            int displayOrder = 1;
            Guid freelancerId = Guid.NewGuid();
            string projectUrl = "www.Project.com";
            var projectResult = PortfolioProject.Create(title, description, PortfolioProjectCategory.BackendDevelopment, freelancerId, ImageUrl, displayOrder, projectUrl);

            Assert.True(projectResult.IsError);
            Assert.Equal(PortfolioProjectErrors.DescriptionIsRequired.Code, projectResult.TopError.Code);
            Assert.Equal(PortfolioProjectErrors.DescriptionIsRequired.Description, projectResult.TopError.Description);
        }

        [Fact]
        public void Create_WithEmptyFreelancerId_ShouldFail()
        {
            string title = "Kawader";
            string description = "A freelancing platform";
            string ImageUrl = "www.image.com";
            int displayOrder = 1;
            Guid freelancerId = Guid.Empty;
            string projectUrl = "www.Project.com";
            var projectResult = PortfolioProject.Create(title, description, PortfolioProjectCategory.BackendDevelopment, freelancerId, ImageUrl, displayOrder, projectUrl);

            Assert.True(projectResult.IsError);
            Assert.Equal(PortfolioProjectErrors.FreelancerIdIsRequired.Code, projectResult.TopError.Code);
            Assert.Equal(PortfolioProjectErrors.FreelancerIdIsRequired.Description, projectResult.TopError.Description);
        }

        [Fact]
        public void Update_WithValidData_ShouldSucceed()
        {
            string title = "Kawader";
            string description = "A freelancing platform";
            string ImageUrl = "www.image.com";
            int displayOrder = 1;
            Guid freelancerId = Guid.NewGuid();
            var projectResult = PortfolioProject.Create(title, description, PortfolioProjectCategory.BackendDevelopment, freelancerId, ImageUrl, displayOrder);

            var project = projectResult.Value;
            var newImageUrl = "www.photo.com";
            var projectUrl = "www.welcome.com";
            var isPublic = false;

            var result = project.Update(projectUrl, newImageUrl, isPublic);
            Assert.True(result.IsSuccess);
            Assert.Equal(newImageUrl, project.ProjectImageUrl);
            Assert.Equal(projectUrl, project.ProjectUrl);
            Assert.Equal(isPublic, project.IsPublic);
        }

        [Fact]
        public void UpdateOrder_ValidOrder_ShouldSucceed()
        {
            string title = "Kawader";
            string description = "A freelancing platform";
            string ImageUrl = "www.image.com";
            int displayOrder = 1;
            Guid freelancerId = Guid.NewGuid();
            var projectResult = PortfolioProject.Create(title, description, PortfolioProjectCategory.BackendDevelopment, freelancerId, ImageUrl, displayOrder);

            var project = projectResult.Value;
            var Order = 5;
            var result = project.UpdateOrder(Order);
            Assert.True(result.IsSuccess);
            Assert.Equal(Order, project.DisplayOrder);
        }
    }
}