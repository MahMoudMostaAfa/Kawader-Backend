using Kawadar.Domain.Jobs.JobReports;
using Kawadar.Domain.Jobs.JobReports.Enums;
using Xunit;

namespace kawadar.Domain.UnitTests.Jobs
{
    public class JobReportTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var result = JobReport.Create(Guid.NewGuid(), Guid.NewGuid(), "Suspicious posting", ReportType.Scam);

            Assert.True(result.IsSuccess);
            Assert.Equal(ReportStatus.Submitted, result.Value.ReportStatus);
            Assert.Equal(ReportType.Scam, result.Value.ReportType);
        }

        [Fact]
        public void Create_WithEmptyJobId_ShouldFail()
        {
            var result = JobReport.Create(Guid.Empty, Guid.NewGuid(), "Reason", ReportType.Scam);

            Assert.True(result.IsError);
            Assert.Equal(JobReportErrors.JobIdIsEmpty.Code, result.TopError.Code);
            Assert.Equal(JobReportErrors.JobIdIsEmpty.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithEmptyReportedById_ShouldFail()
        {
            var result = JobReport.Create(Guid.NewGuid(), Guid.Empty, "Reason", ReportType.Scam);

            Assert.True(result.IsError);
            Assert.Equal(JobReportErrors.ReportedByIdIsEmpty.Code, result.TopError.Code);
            Assert.Equal(JobReportErrors.ReportedByIdIsEmpty.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithEmptyContent_ShouldFail()
        {
            var result = JobReport.Create(Guid.NewGuid(), Guid.NewGuid(), string.Empty, ReportType.Scam);

            Assert.True(result.IsError);
            Assert.Equal(JobReportErrors.ContentIsEmpty.Code, result.TopError.Code);
            Assert.Equal(JobReportErrors.ContentIsEmpty.Description, result.TopError.Description);
        }

        [Fact]
        public void Update_WithValidData_ShouldSucceed()
        {
            var report = JobReport.Create(Guid.NewGuid(), Guid.NewGuid(), "Reason", ReportType.Scam).Value;

            var result = report.Update(ReportStatus.Resolved, "Removed job");

            Assert.True(result.IsSuccess);
            Assert.Equal(ReportStatus.Resolved, report.ReportStatus);
            Assert.Equal("Removed job", report.ActionTaken);
        }
    }
}
