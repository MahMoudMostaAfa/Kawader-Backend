using Kawadar.Domain.Common.Results;


namespace Kawadar.Domain.UserProfiles.UserReports
{
    public class UserReportErrors
    {
        public static Error ReportedByIsEmpty => Error.Validation("Reported By can't be empty");
        public static Error ReportedUserIsEmpty => Error.Validation("Reported User can't be empty");
        public static Error ContentIsEmpty => Error.Validation("Content can't be empty");

    }
}
