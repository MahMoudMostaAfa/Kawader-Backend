using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Queries.GetRecommendationJobs;

public class GetRecommandationJobsQueryHandler : IRequestHandler<GetRecommandationJobsQuery, Result<PaginatedList<JobSummaryDto>>>
{
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;

  private readonly IJobsRepository _jobsRepository;
  private readonly IRecommendationService _recommendationService;
  private readonly IMapper _mapper;

  public GetRecommandationJobsQueryHandler(IUser user, IUsersRepository usersRepository
  , IRecommendationService recommendationService
  , IJobsRepository jobsRepository, IMapper mapper
  )
  {
    _user = user;
    _usersRepository = usersRepository;
    _recommendationService = recommendationService;
    _jobsRepository = jobsRepository;
    _mapper = mapper;
  }

  public async Task<Result<PaginatedList<JobSummaryDto>>> Handle(GetRecommandationJobsQuery request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UnauthorizedAccess;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var jobsIdsResult = await _recommendationService.GetRecommendationsAsync(userProfile.Id, request.page, request.pageSize, cancellationToken);

    var jobIds = jobsIdsResult.Value.Items;

    var jobsResult = await _jobsRepository.GetJobsByIds(jobIds);
    if (jobsResult.IsError) return jobsResult.Errors;

    var jobs = jobsResult.Value;


    var jobDtos = jobs.Select(j => _mapper.Map<JobSummaryDto>(j)).ToList();

    var paginatedResult = new PaginatedList<JobSummaryDto>(
        jobDtos,
        jobsIdsResult.Value.TotalCount,
        request.page,
        request.pageSize
    );



    return paginatedResult;


  }
}