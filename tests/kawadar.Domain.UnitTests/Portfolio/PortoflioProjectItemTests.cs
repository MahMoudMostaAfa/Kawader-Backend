using Kawadar.Domain.Portfolios.Items;
using Kawadar.Domain.Portfolios.Items.Enum;
using Xunit;

namespace kawadar.Domain.UnitTests.Portfolio
{
    public class PortoflioProjectItemTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            string content = "This is the content of an Item";
            int displayOrder = 1;
            Guid projectId = Guid.NewGuid();

            var result = PortfolioItem.Create(ItemType.Text, content, displayOrder, projectId);
            Assert.True(result.IsSuccess);
            var item = result.Value;
            Assert.Equal(content, item.Content);
            Assert.Equal(projectId, item.PortfolioProjectId);
            Assert.Equal(displayOrder, item.DisplayOrder);
            Assert.Equal(ItemType.Text, item.ItemType);
        }

        [Fact]
        public void Create_EmptyContent_ShouldFail()
        {
            string content = "";
            int displayOrder = 1;
            Guid projectId = Guid.NewGuid();

            var result = PortfolioItem.Create(ItemType.Text, content, displayOrder, projectId);
            Assert.True(result.IsError);
            Assert.Equal(PortfolioItemErrors.ContentIsRequired.Code, result.TopError.Code);
            Assert.Equal(PortfolioItemErrors.ContentIsRequired.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_EmptyProjectId_ShouldFail()
        {
            string content = "This is the content of an Item";
            int displayOrder = 1;
            Guid projectId = Guid.Empty;

            var result = PortfolioItem.Create(ItemType.Text, content, displayOrder, projectId);
            Assert.True(result.IsError);
            Assert.Equal(PortfolioItemErrors.PortfolioIdRequired.Code, result.TopError.Code);
            Assert.Equal(PortfolioItemErrors.PortfolioIdRequired.Description, result.TopError.Description);
        }

        [Fact]
        public void Update_ValidData_ShouldSucceed()
        {
            string content = "This is the content of an Item";
            int displayOrder = 1;
            Guid projectId = Guid.NewGuid();

            var result = PortfolioItem.Create(ItemType.Text, content, displayOrder, projectId);
            Assert.True(result.IsSuccess);
            var item = result.Value;

            string updateContent = "THis is updated content";
            var updateResult = item.Update(updateContent);
            Assert.True(updateResult.IsSuccess);
            Assert.Equal(updateContent, item.Content);
        }

        [Fact]
        public void UpdateDisplayOrder_ValidOrder_ShouldSucceed()
        {
            string content = "This is the content of an Item";
            int displayOrder = 1;
            Guid projectId = Guid.NewGuid();

            var result = PortfolioItem.Create(ItemType.Text, content, displayOrder, projectId);
            Assert.True(result.IsSuccess);
            var item = result.Value;
            var order = 5;
            var updateResult = item.UpdateDisplayOrder(order);
            Assert.True(updateResult.IsSuccess);
            Assert.Equal(order, item.DisplayOrder);
        }
    }
}
