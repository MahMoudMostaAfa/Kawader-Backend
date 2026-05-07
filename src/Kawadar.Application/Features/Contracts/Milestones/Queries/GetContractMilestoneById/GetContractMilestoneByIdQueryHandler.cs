using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Contracts.Dtos;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Milestones.Queries.GetContractMilestoneById;

public class GetContractMilestoneByIdQueryHandler : IRequestHandler<GetContractMilestoneByIdQuery, Result<ContractMilestoneDto>>
{
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IContractsRepository _contractsRepository;

  public GetContractMilestoneByIdQueryHandler(IUser user, IUsersRepository usersRepository, IContractsRepository contractsRepository)
  {
    _user = user;
    _usersRepository = usersRepository;
    _contractsRepository = contractsRepository;
  }

  public async Task<Result<ContractMilestoneDto>> Handle(GetContractMilestoneByIdQuery request, CancellationToken cancellationToken)
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

    var milestone = contract.ContractMilestones.FirstOrDefault(m => m.Id == request.MilestoneId);
    if (milestone is null)
      return Error.NotFound("Contracts.Milestones", "Milestone not found.");

    var milestoneDto = new ContractMilestoneDto
    {
      Id = milestone.Id,
      ProposalMilestoneId = milestone.ProposalMilestoneId,
      Title = milestone.Title,
      Description = milestone.Description,
      Amount = milestone.Amount,
      DueDate = milestone.DueDate,
      CompletionRequestedAt = milestone.CompletionRequestedAt,
      CompletionApprovedAt = milestone.CompletionApprovedAt,
      RejectionReason = milestone.RejectionReason,
      Order = milestone.Order,
      Status = milestone.Status
    };

    return milestoneDto;
  }
}
