

using Kawadar.Domain.Jobs.JobReports.Enums;

namespace Kawadar.Application.Features.ProfileManagment.DTOs
{
    public class FullUserReportDto
    {
        public Guid Id { get; set; }
        public string ReportedUserName { get; set; } = "";
        public string ReporterUserName { get; set; } = "";
        public ReportStatus reportStatus { get; set; }
        public ReportType reportType { get; set; }
        public string ActionTaken { get; set; } = "";
        public string Content { get; set; } = "";
    }
}
