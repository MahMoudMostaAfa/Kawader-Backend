using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobReports.Enums;

namespace Kawadar.Domain.UserProfiles.UserReports
{
    public class UserReport : AuditableEntity
    {
        public Guid ReportedUser { get; private set; }
        public Guid ReportedBy { get; private set; }
        public ReportType ReportType { get; private set; }
        public ReportStatus ReportStatus { get; private set; }
        public string Content { get; private set; } = "";
        public string ActionTaken { get; private set; } = "";

        private UserReport(Guid ReportedUser, Guid ReportedBy, ReportType ReportType, string Content)
        {
            this.ReportedUser = ReportedUser;
            this.ReportedBy = ReportedBy;
            this.ReportType = ReportType;
            this.Content = Content;
            ReportStatus = ReportStatus.Submitted;
        }

        public static Result<UserReport> Create(Guid ReportedUser, Guid ReportedBy, ReportType ReportType, string Content)
        {
            if(ReportedUser == Guid.Empty)
            {
                return UserReportErrors.ReportedUserIsEmpty;
            }

            if(ReportedBy == Guid.Empty)
            {
                return UserReportErrors.ReportedByIsEmpty;
            }

            if (string.IsNullOrEmpty(Content))
            {
                return UserReportErrors.ContentIsEmpty;
            }

            var Report = new UserReport(ReportedUser, ReportedBy, ReportType, Content);
            return Report;
        }

        public Result<Updated> Update(ReportStatus ReportStatus, string ActionTaken)
        {
            this.ReportStatus = ReportStatus;
            this.ActionTaken = ActionTaken;
            UpdatedAt = DateTime.UtcNow;

            return Result.Updated;
        }
    }
}
