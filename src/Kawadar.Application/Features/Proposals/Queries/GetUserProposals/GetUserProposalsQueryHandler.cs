using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Proposals.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Proposals.Queries.GetUserProposals;


public class GetUserProposalsQueryHandler : IRequestHandler<GetUserProposalsQuery, Result<PaginatedList<ProposalSummaryDto>>>
{

  private readonly IUser _user;

  private readonly IUsersRepository _usersRepository;
  private readonly IProposalsRepository _proposalsRepository;


  public GetUserProposalsQueryHandler(IUser user, IUsersRepository usersRepository, IProposalsRepository proposalsRepository)
  {
    _user = user;
    _proposalsRepository = proposalsRepository;
    _usersRepository = usersRepository;
  }
  public async Task<Result<PaginatedList<ProposalSummaryDto>>> Handle(GetUserProposalsQuery request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;

    var profile = userProfileResult.Value;

    var proposalsResult = await _proposalsRepository.GetFreelancerProposals(
      profile.Id,
      request.PageNumber,
      request.PageSize,
      request.SortBy
    );

    var proposals = proposalsResult.Value;

    var proposalSummaryDtos = proposals.Items.Select(p =>
    {


      return new ProposalSummaryDto
      {

        Id = p.Id,
        CoverLetter = p.CoverLetter,
        ProposalType = p.ProposalType,
        ProposedPrice = p.ProposalType switch
        {
          Domain.Proposals.Enums.JobProposalType.OneTime => (decimal)p.Amount!,
          Domain.Proposals.Enums.JobProposalType.Hourly => (decimal)(p.EstimatedHours * p.HourlyRate)!,
          Domain.Proposals.Enums.JobProposalType.MilestoneBased => p.Milestones.Sum(ms => ms.Amount),
          _ => 0m
        },

        CreatedAt = p.CreatedAt,
        TotalMilestones = p.ProposalType == Domain.Proposals.Enums.JobProposalType.MilestoneBased ? p.Milestones.Count() : null,

        EstimatedTimeInDays = p.ProposalType == Domain.Proposals.Enums.JobProposalType.OneTime ? p.EstimatedDays : null,

        EstimatedTimeInHours = p.ProposalType == Domain.Proposals.Enums.JobProposalType.Hourly ? p.EstimatedHours : null,

      }
      ;


    }).ToList();


    return new PaginatedList<ProposalSummaryDto>(proposalSummaryDtos, proposals.TotalCount, request.PageNumber, request.PageSize);
  }
}