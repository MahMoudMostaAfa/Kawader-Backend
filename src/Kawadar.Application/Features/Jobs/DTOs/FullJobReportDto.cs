using Kawadar.Domain.Jobs.JobReports.Enums;

namespace Kawadar.Application.Features.Jobs.DTOs
{
    public class FullJobReportDto
    {
        public Guid Id { get; set; }
        public string JobSlug { get; private set; } = "";
        public string UserName { get; private set; } = "";
        public string Content { get; private set; } = "";
        public string ActionTaken { get; private set; } = "";
        public ReportStatus ReportStatus { get; private set; } = ReportStatus.Submitted;
        public ReportType ReportType { get; private set; } = ReportType.Scam;
    }
}