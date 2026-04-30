using Kawadar.Domain.Jobs.JobReports.Enums;

namespace Kawadar.Application.Features.Jobs.DTOs
{
    public class BriefJobReportDto
    {
        public Guid Id { get; set; }
        public string JobTitle { get; private set; } = "";
        public string ReporterUserName { get; private set; } = "";
        public ReportStatus ReportStatus { get; private set; } = ReportStatus.Submitted;
        public ReportType ReportType { get; private set; } = ReportType.Scam;
    }
}
