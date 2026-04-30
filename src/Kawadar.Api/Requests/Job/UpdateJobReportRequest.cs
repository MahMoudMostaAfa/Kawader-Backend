using Kawadar.Domain.Jobs.JobReports.Enums;

namespace Kawadar.Api.Requests.Job
{
    public class UpdateJobReportRequest
    {
        public ReportStatus reportStatus { get; set; }
        public string ActionTaken { get; set; } = "";
    }
}
