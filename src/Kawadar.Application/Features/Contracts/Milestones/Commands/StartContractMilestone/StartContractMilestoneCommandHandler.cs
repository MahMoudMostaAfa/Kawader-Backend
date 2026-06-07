using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Contracts.Enums;
using Kawadar.Domain.WalletAndPayments;
using Kawadar.Domain.WalletAndPayments.Enums;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Milestones.Commands.StartContractMilestone;

public class StartContractMilestoneCommandHandler : IRequestHandler<StartContractMilestoneCommand, Result<Updated>>
{
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IContractsRepository _contractsRepository;
  private readonly IWalletRepository _walletRepository;
  private readonly IUnitOfWork _unitOfWork;

  public StartContractMilestoneCommandHandler(IUser user, IUsersRepository usersRepository, IContractsRepository contractsRepository, IWalletRepository walletRepository, IUnitOfWork unitOfWork)
  {
    _user = user;
    _usersRepository = usersRepository;
    _contractsRepository = contractsRepository;
    _walletRepository = walletRepository;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<Updated>> Handle(StartContractMilestoneCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;

    var contractResult = await _contractsRepository.GetContractByIdAsync(request.ContractId, cancellationToken);
    if (contractResult.IsError) return contractResult.Errors;
    var contract = contractResult.Value;

    if (contract.ClientId != userProfile.Id)
      return ApplicationErrors.UnauthorizedAccess;

    var milestone = contract.ContractMilestones.FirstOrDefault(m => m.Id == request.MilestoneId);
    if (milestone is null)
      return Error.NotFound("Contracts.Milestones", "Milestone not found.");

    if (contract.Type != ContractType.MilestoneBased)
      return Error.Conflict("Contracts.Milestones", "Milestones can only be started for milestone-based contracts.");

    if (contract.Status != ContractStatus.Active)
      return Error.Conflict("Contracts.Milestones", "Only active contracts can start milestones.");

    if (milestone.Status != ContractMilestoneStatus.Pending)
      return Error.Conflict("Contracts.Milestones", "Only pending milestones can be started.");

    var previousMilestone = contract.ContractMilestones
      .Where(m => m.Order < milestone.Order)
      .OrderByDescending(m => m.Order)
      .FirstOrDefault();

    if (previousMilestone is not null && previousMilestone.Status != ContractMilestoneStatus.Approved)
      return Error.Conflict("Contracts.Milestones", "Previous milestone must be approved before starting this milestone.");

    var existingEscrowResult = await _walletRepository.GetEscrowTransactionByContractMilestoneId(milestone.Id, cancellationToken);
    if (!existingEscrowResult.IsError)
      return Error.Conflict("Contracts.Milestones", "Milestone escrow is already funded.");

    if (existingEscrowResult.TopError.Type != ErrorKind.NotFound)
      return existingEscrowResult.Errors;

    var startResult = contract.StartMilestone(request.MilestoneId);
    if (startResult.IsError) return startResult.Errors;

    var clientWalletResult = await _walletRepository.GetByUserIdAsync(contract.ClientId, cancellationToken);
    if (clientWalletResult.IsError) return clientWalletResult.Errors;
    var clientWallet = clientWalletResult.Value;

    var holdResult = clientWallet.AddTransaction(
      milestone.Amount,
      TransactionType.EscrowHold,
      WalletTransactionReferenceType.Contract,
      contract.Id,
      null);
    if (holdResult.IsError) return holdResult.Errors;

    var escrowResult = EscrowTransaction.Create(
      contract.Id,
      milestone.Id,
      EcrowTransactionType.Hold,
      milestone.Amount,
      contract.ClientId,
      contract.FreelancerId,
      null);
    if (escrowResult.IsError) return escrowResult.Errors;

    _walletRepository.AddEscrowTransaction(escrowResult.Value);

    await _unitOfWork.SaveChangesAsync(cancellationToken);
    return Result.Updated;
  }
}
