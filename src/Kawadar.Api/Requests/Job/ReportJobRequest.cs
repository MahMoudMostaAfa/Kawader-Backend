using Kawadar.Domain.Jobs.JobReports.Enums;

namespace Kawadar.Api.Requests.Job
{
    public class ReportJobRequest
    {
        public ReportType reportType { get; set; }
        public string content { get; set; } = "";
    }
}
