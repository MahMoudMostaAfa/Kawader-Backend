
using Kawadar.Domain.Badges;
using Xunit;

namespace kawadar.Domain.UnitTests.Badges
{
    public class BadgeTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var title = "Badge";
            var iconUrl = "www.Icon.com";
            var description = "this is a badge";

            var result = Badge.Create(title, iconUrl, description);
            Assert.True(result.IsSuccess);
            var badge = result.Value;
            Assert.Equal(title, badge.Title);
            Assert.Equal(iconUrl, badge.IconUrl);
            Assert.Equal(description, badge.Description);
        }

        [Fact]
        public void Create_WithEmptyTitle_ShouldFail()
        {
            var title = "";
            var iconUrl = "www.Icon.com";
            var description = "this is a badge";

            var result = Badge.Create(title, iconUrl, description);
            Assert.True(result.IsError);
            Assert.Equal(BadgeErrors.TitleIsEmpty.Code, result.TopError.Code);
            Assert.Equal(BadgeErrors.TitleIsEmpty.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithNullTitle_ShouldFail()
        {
            string? title = null;
            var iconUrl = "www.Icon.com";
            var description = "this is a badge";

            var result = Badge.Create(title!, iconUrl, description);
            Assert.True(result.IsError);
            Assert.Equal(BadgeErrors.TitleIsEmpty.Code, result.TopError.Code);
            Assert.Equal(BadgeErrors.TitleIsEmpty.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithEmptyIconUrl_ShouldFail()
        {
            var title = "Badge";
            var iconUrl = "";
            var description = "this is a badge";

            var result = Badge.Create(title, iconUrl, description);
            Assert.True(result.IsError);
            Assert.Equal(BadgeErrors.IconIsEmpty.Code, result.TopError.Code);
            Assert.Equal(BadgeErrors.IconIsEmpty.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithNullIconUrl_ShouldFail()
        {
            var title = "Badge";
            string? iconUrl = null;
            var description = "this is a badge";

            var result = Badge.Create(title, iconUrl!, description);
            Assert.True(result.IsError);
            Assert.Equal(BadgeErrors.IconIsEmpty.Code, result.TopError.Code);
            Assert.Equal(BadgeErrors.IconIsEmpty.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithEmptyDescription_ShouldFail()
        {
            var title = "Badge";
            var iconUrl = "www.Icon.com";
            var description = "";

            var result = Badge.Create(title, iconUrl, description);
            Assert.True(result.IsError);
            Assert.Equal(BadgeErrors.DescriptionIsEmpty.Code, result.TopError.Code);
            Assert.Equal(BadgeErrors.DescriptionIsEmpty.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithNullDescription_ShouldFail()
        {
            var title = "Badge";
            var iconUrl = "www.Icon.com";
            string? description = null;

            var result = Badge.Create(title, iconUrl, description!);
            Assert.True(result.IsError);
            Assert.Equal(BadgeErrors.DescriptionIsEmpty.Code, result.TopError.Code);
            Assert.Equal(BadgeErrors.DescriptionIsEmpty.Description, result.TopError.Description);
        }

        [Fact]
        public void Update_WithValidData_ShouldSucceed()
        {
            var title = "Badge";
            var iconUrl = "www.Icon.com";
            var description = "this is a badge";

            var result = Badge.Create(title, iconUrl, description);
            var badge = result.Value;
            var newUrl = "www.Url.com";
            var updateResult = badge.Update(newUrl);
            Assert.True(updateResult.IsSuccess);
            Assert.Equal(newUrl, badge.IconUrl);
        }
    }
}
