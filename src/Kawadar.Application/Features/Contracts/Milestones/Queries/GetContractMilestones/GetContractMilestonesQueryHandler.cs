using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Contracts.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Milestones.Queries.GetContractMilestones;

public class GetContractMilestonesQueryHandler : IRequestHandler<GetContractMilestonesQuery, Result<List<ContractMilestoneDto>>>
{
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IContractsRepository _contractsRepository;

  public GetContractMilestonesQueryHandler(IUser user, IUsersRepository usersRepository, IContractsRepository contractsRepository)
  {
    _user = user;
    _usersRepository = usersRepository;
    _contractsRepository = contractsRepository;
  }

  public async Task<Result<List<ContractMilestoneDto>>> Handle(GetContractMilestonesQuery request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var contractResult = await _contractsRepository.GetContractByIdAsync(request.ContractId, cancellationToken);
    if (contractResult.IsError) return contractResult.Errors;
    var contract = contractResult.Value;

    if (contract.ClientId != userProfile.Id && contract.FreelancerId != userProfile.Id)
      return ApplicationErrors.UnauthorizedAccess;

    var milestones = contract.ContractMilestones
      .OrderBy(m => m.Order)
      .Select(m => new ContractMilestoneDto
      {
        Id = m.Id,
        ProposalMilestoneId = m.ProposalMilestoneId,
        Title = m.Title,
        Description = m.Description,
        Amount = m.Amount,
        DueDate = m.DueDate,
        CompletionRequestedAt = m.CompletionRequestedAt,
        CompletionApprovedAt = m.CompletionApprovedAt,
        RejectionReason = m.RejectionReason,
        Order = m.Order,
        Status = m.Status
      })
      .ToList();

    return milestones;
  }
}
