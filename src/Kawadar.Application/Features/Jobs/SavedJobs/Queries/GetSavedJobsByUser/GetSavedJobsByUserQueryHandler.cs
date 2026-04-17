using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Application.Features.Jobs.SavedJobs.Queries.GetSavedJobsByUser;


public class GetSavedJobsByUserQueryHandler : IRequestHandler<GetSavedJobsByUserQuery, Result<PaginatedList<JobSummaryDto>>>
{

  private readonly IUser _user;
  private readonly ISavedJobsRepository _savedJobsRepository;
  private readonly IUsersRepository _usersRepository;
  private readonly ILogger<GetSavedJobsByUserQueryHandler> _logger;
  private readonly IMapper _mapper;


  public GetSavedJobsByUserQueryHandler(IUser user, ISavedJobsRepository savedJobsRepository, IUsersRepository usersRepository, ILogger<GetSavedJobsByUserQueryHandler> logger, IMapper mapper)
  {
    _user = user;
    _savedJobsRepository = savedJobsRepository;
    _usersRepository = usersRepository;
    _logger = logger;
    _mapper = mapper;

  }

  public async Task<Result<PaginatedList<JobSummaryDto>>> Handle(GetSavedJobsByUserQuery request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);

    if (userProfileResult.IsError) return userProfileResult.Errors;

    var userProfile = userProfileResult.Value;


    var savedJobsResult = await _savedJobsRepository.GetSavedJobsbyUserIdAsync(userProfile.Id, request.PageNumber, request.PageSize);
    if (savedJobsResult.IsError) return savedJobsResult.Errors;

    var savedJobs = savedJobsResult.Value;

    var savedJobDtos = savedJobs.Items.Select(si => _mapper.Map<JobSummaryDto>(si.Job)).ToList();

    var paginatedSavedJobDtos = new PaginatedList<JobSummaryDto>(savedJobDtos, savedJobs.TotalCount, request.PageNumber, request.PageSize);

    return paginatedSavedJobDtos;


  }
}