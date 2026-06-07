using Kawadar.Domain.Common.Results;
using System.Reflection.Metadata.Ecma335;

namespace Kawadar.Domain.Jobs.JobReports
{
    public class JobReportErrors
    {
        public static Error JobIdIsEmpty => Error.Validation("Job Id can't be empty");
        public static Error ReportedByIdIsEmpty => Error.Validation("The reporter Id can't be empty");
        public static Error ContentIsEmpty => Error.Validation("Content can't be empty");
    }
}
