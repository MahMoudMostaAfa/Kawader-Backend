using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobReports.Enums;

namespace Kawadar.Domain.Jobs.JobReports
{
    public class JobReport : AuditableEntity
    {
        public Guid JobId { get; private set; }
        public Guid ReportedBy { get; private set; }
        public string Content { get; private set; } = "";
        public string ActionTaken { get; private set; } = "";
        public ReportStatus ReportStatus { get; private set; } = ReportStatus.Submitted;
        public ReportType ReportType { get; private set; } = ReportType.Scam;

        private JobReport(Guid jobId, Guid reportedBy, string content, ReportType reportType)
        {
            JobId = jobId;
            ReportedBy = reportedBy;
            Content = content;
            ReportType = reportType;
        }

        public static Result<JobReport> Create(Guid jobId, Guid reportedBy, string content, ReportType reportType)
        {
            if (jobId.Equals(Guid.Empty)) return JobReportErrors.JobIdIsEmpty;
            if (reportedBy.Equals(Guid.Empty)) return JobReportErrors.ReportedByIdIsEmpty;
            if (string.IsNullOrEmpty(content)) return JobReportErrors.ContentIsEmpty;

            var report = new JobReport(jobId, reportedBy, content, reportType);
            report.ReportStatus = ReportStatus.Submitted;
            return report;
        }

        public Result<Updated> Update(ReportStatus reportStatus, string actionTaken)
        {
            ReportStatus = reportStatus;
            ActionTaken = actionTaken;

            UpdatedAt = DateTime.UtcNow;
            return Result.Updated;
        }
    }
}
