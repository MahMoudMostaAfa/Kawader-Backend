using Kawadar.Domain.Jobs.Enums;

namespace Kawadar.Application.Features.Admins.Dtos
{
    public class JobsStatisticsDto
    {
        public int totalJobCount { get; set; }
        public Dictionary<JobStatus, int>? DistributionBasedOnStatus { get; set; }
        public Dictionary<string, int>? DistributionBasedOnSpecilization { get; set; }
        public Dictionary<int, int>? DistributionBasedOnMonth { get; set; }
    }
}
