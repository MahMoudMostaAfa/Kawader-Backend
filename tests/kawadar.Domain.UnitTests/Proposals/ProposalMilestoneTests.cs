using Kawadar.Domain.Proposals.ProposalMilestones;
using Kawadar.Domain.Common.Results;
using Kawadar.Tests.Common.Proposals;
using Xunit;

namespace kawadar.Domain.UnitTests.Proposals
{
    public class ProposalMilestoneTests
    {
        // Create
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var result = ProposalMilestoneFactory.Builder().BuildResult();

            Assert.True(result.IsSuccess);
            Assert.Equal(ProposalMilestoneStatus.Pending, result.Value.Status);
        }

        [Fact]
        public void Create_WithEmptyTitle_ShouldFail()
        {
            var result = ProposalMilestoneFactory.Builder().WithTitle(string.Empty).BuildResult();
            var expectedError = Error.Validation("Title is required.");

            Assert.True(result.IsError);
            Assert.Equal(expectedError.Code, result.TopError.Code);
            Assert.Equal(expectedError.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithZeroAmount_ShouldFail()
        {
            var result = ProposalMilestoneFactory.Builder().WithAmount(0m).BuildResult();
            var expectedError = Error.Validation("Amount must be greater than zero.");

            Assert.True(result.IsError);
            Assert.Equal(expectedError.Code, result.TopError.Code);
            Assert.Equal(expectedError.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithPastDueDate_ShouldFail()
        {
            var result = ProposalMilestoneFactory.Builder().WithDueDate(DateTime.UtcNow.AddMinutes(-1)).BuildResult();
            var expectedError = Error.Validation("Due date must be in the future.");

            Assert.True(result.IsError);
            Assert.Equal(expectedError.Code, result.TopError.Code);
            Assert.Equal(expectedError.Description, result.TopError.Description);
        }

        // Update
        [Fact]
        public void Update_WithValidData_ShouldSucceed()
        {
            var milestone = ProposalMilestoneFactory.CreateValid();

            var result = milestone.Update("new title", "new desc", 300m, DateTime.UtcNow.AddDays(20), ProposalMilestoneStatus.InProgress, 3);

            Assert.True(result.IsSuccess);
            Assert.Equal(ProposalMilestoneStatus.InProgress, milestone.Status);
            Assert.Equal(3, milestone.DisplayOrder);
        }

        [Fact]
        public void Update_WithInvalidValues_ShouldKeepOriginalValues()
        {
            var milestone = ProposalMilestoneFactory.CreateValid();
            var originalTitle = milestone.Title;
            var originalAmount = milestone.Amount;

            var result = milestone.Update(string.Empty, null, -5m, DateTime.UtcNow.AddMinutes(-1), null, null);

            Assert.True(result.IsSuccess);
            Assert.Equal(originalTitle, milestone.Title);
            Assert.Equal(originalAmount, milestone.Amount);
        }
    }
}
