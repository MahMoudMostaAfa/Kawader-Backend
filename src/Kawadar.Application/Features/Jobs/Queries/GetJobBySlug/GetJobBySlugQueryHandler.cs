using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Queries.GetJobBySlug;

public class GetJobBySlugQueryHandler : IRequestHandler<GetJobBySlugQuery, Result<JobDetailsDto>>
{
  private readonly IJobsRepository _jobsRepository;
  private readonly IMapper _mapper;
  private readonly IIdentityService _identityService;
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;

  public GetJobBySlugQueryHandler(IJobsRepository jobsRepository, IMapper mapper, IIdentityService identityService, IUser user, IUsersRepository usersRepository)
  {
    _jobsRepository = jobsRepository;
    _mapper = mapper;
    _identityService = identityService;
    _user = user;
    _usersRepository = usersRepository;


  }
  public async Task<Result<JobDetailsDto>> Handle(GetJobBySlugQuery request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var jobResult = await _jobsRepository.GetJobBySlugAsync(request.Slug);
    if (jobResult.IsError) return jobResult.Errors;

    var job = jobResult.Value;

    var UserProfileResult = await _usersRepository.GetUserProfileByIdAsync(job.PostedById);
    if (UserProfileResult.IsError) return UserProfileResult.Errors;
    var userProfile = UserProfileResult.Value;

    var posterIdentityResult = await _identityService.GetUserByIdAsync(userProfile.UserId);
    if (posterIdentityResult.IsError) return posterIdentityResult.Errors;
    var posterIdentity = posterIdentityResult.Value;

    var jobDetailsDto = _mapper.Map<JobDetailsDto>((job, userProfile, posterIdentity));

    return jobDetailsDto;

  }
}