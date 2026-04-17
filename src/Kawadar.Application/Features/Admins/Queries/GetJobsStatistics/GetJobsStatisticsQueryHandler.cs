using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Admins.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Admins.Queries.GetJobsStatistics
{
    public class GetJobsStatisticsQueryHandler(IUser user, IJobsRepository jobsRepository) : IRequestHandler<GetJobsStatisticsQuery, Result<JobsStatisticsDto>>
    {
        public async Task<Result<JobsStatisticsDto>> Handle(GetJobsStatisticsQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var DistributionBasedOnStatus = await jobsRepository.GetJobStatusDistribution();
            if (DistributionBasedOnStatus.IsError) return DistributionBasedOnStatus.Errors;

            var DistributionBasedOnMonth = await jobsRepository.GetAverageJobPostingPerMonthDistribution();
            if (DistributionBasedOnMonth.IsError) return DistributionBasedOnMonth.Errors;

            var DistributionBasedOnSpecilization = await jobsRepository.GetJobSpecilizationDistribution();
            if (DistributionBasedOnSpecilization.IsError) return DistributionBasedOnSpecilization.Errors;

            var TotalJobCount = DistributionBasedOnStatus.Value.Values.Sum();

            var jobStatisticsDto = new JobsStatisticsDto
            {
                totalJobCount = TotalJobCount,
                DistributionBasedOnStatus = DistributionBasedOnStatus.Value,
                DistributionBasedOnMonth = DistributionBasedOnMonth.Value,
                DistributionBasedOnSpecilization = DistributionBasedOnSpecilization.Value
            };

            return jobStatisticsDto;
        }
    }
}
