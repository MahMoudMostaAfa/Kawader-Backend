using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Proposals.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Proposals.Queries.GetProposalById;

public class GetProposalByIdQueryHandler : IRequestHandler<GetProposalByIdQuery, Result<ProposalDetailsDto>>
{
  private readonly IUser _user;
  private readonly IProposalsRepository _proposalsRepository;
  private readonly IJobsRepository _jobsRepository;
  private readonly IUsersRepository _usersRepository;
  private readonly IIdentityService _identityService;

  private readonly IMapper _mapper;
  public GetProposalByIdQueryHandler(IUser user, IProposalsRepository proposalsRepository, IJobsRepository jobsRepository, IUsersRepository usersRepository, IMapper mapper, IIdentityService identityService)
  {
    _user = user;
    _proposalsRepository = proposalsRepository;
    _jobsRepository = jobsRepository;
    _usersRepository = usersRepository;
    _mapper = mapper;
    _identityService = identityService;
  }

  public async Task<Result<ProposalDetailsDto>> Handle(GetProposalByIdQuery request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var proposalResult = await _proposalsRepository.GetDetailsByIdAsync(request.ProposalId);
    if (proposalResult.IsError) return proposalResult.Errors;

    var proposal = proposalResult.Value;
    var JobResult = await _jobsRepository.GetJobByIdAsync(proposal.JobId);

    if (JobResult.IsError) return JobResult.Errors;

    var Job = JobResult.Value;

    // Check that the user is posted the proposal or Job Poster 
    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;
    if (proposal.FreelancerId != userProfile.Id && Job.PostedById != userProfile.Id) return Error.Unauthorized();
    // get user identiy 
    var userResult = await _identityService.GetUserByIdAsync(userId);

    if (userResult.IsError) return userResult.Errors;

    var user = userResult.Value;

    var proposalMappingResult = _mapper.Map<ProposalDetailsDto>((proposal, user, userProfile));
    if (proposal.FreelancerId == userProfile.Id)
    {
      proposalMappingResult.ProposalByUserName = null;
      proposalMappingResult.ProposalByPhoto = null;
      proposalMappingResult.ProposalByFullName = null;
    }



    return proposalMappingResult;


  }
}