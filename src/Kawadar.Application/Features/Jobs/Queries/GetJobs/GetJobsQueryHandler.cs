using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Queries.GetJobs;

public class GetJobsQueryHandler : IRequestHandler<GetJobsQuery, Result<PaginatedList<JobSummaryDto>>>
{
  private readonly IJobsRepository _jobsRepository;
  private readonly IMapper _mapper;
  private readonly IUser _user;

  public GetJobsQueryHandler(IJobsRepository jobsRepository, IMapper mapper, IUser user)
  {
    _jobsRepository = jobsRepository;
    _mapper = mapper;
    _user = user;
  }

  public async Task<Result<PaginatedList<JobSummaryDto>>> Handle(GetJobsQuery request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var result = await _jobsRepository.GetJobsAsync(
      request.Search,
      request.SpecilizationId,
      request.JobType,
      request.ExperienceLevel,
      request.BudgetRange,
      request.HourlyRateRange,
      request.SkillIds,
      request.Page,
      request.PageSize,
      request.SortBy
    );

    var jobs = result.Items.Select(job => _mapper.Map<JobSummaryDto>(job)).ToList();

    return new PaginatedList<JobSummaryDto>(jobs, result.TotalCount, result.PageNumber, request.PageSize);
  }
}
