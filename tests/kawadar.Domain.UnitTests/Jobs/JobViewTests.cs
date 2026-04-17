using Kawadar.Domain.Jobs.JobViews;
using Xunit;

namespace kawadar.Domain.UnitTests.Jobs
{
    public class JobViewTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var jobId = Guid.NewGuid();
            var userProfileId = Guid.NewGuid();

            var result = JobView.Create(jobId, userProfileId);

            Assert.True(result.IsSuccess);
            Assert.Equal(jobId, result.Value.JobId);
            Assert.Equal(userProfileId, result.Value.UserProfileId);
        }

        [Fact]
        public void Create_WithEmptyJobId_ShouldFail()
        {
            var result = JobView.Create(Guid.Empty, Guid.NewGuid());

            Assert.True(result.IsError);
            Assert.Equal(JobViewErrors.JobIdIsRequired.Code, result.TopError.Code);
            Assert.Equal(JobViewErrors.JobIdIsRequired.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithEmptyUserProfileId_ShouldFail()
        {
            var result = JobView.Create(Guid.NewGuid(), Guid.Empty);

            Assert.True(result.IsError);
            Assert.Equal(JobViewErrors.UserProfileIdIsRequired.Code, result.TopError.Code);
            Assert.Equal(JobViewErrors.UserProfileIdIsRequired.Description, result.TopError.Description);
        }
    }
}
