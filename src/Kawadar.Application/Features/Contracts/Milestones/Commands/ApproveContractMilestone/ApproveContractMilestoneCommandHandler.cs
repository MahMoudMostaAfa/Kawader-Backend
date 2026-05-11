using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.BackgroundJobs;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Contracts.Enums;
using Kawadar.Domain.Jobs.Enums;
using Kawadar.Domain.WalletAndPayments;
using Kawadar.Domain.WalletAndPayments.Enums;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Milestones.Commands.ApproveContractMilestone;

public class ApproveContractMilestoneCommandHandler : IRequestHandler<ApproveContractMilestoneCommand, Result<Updated>>
{
  private readonly IUser _user;
  private readonly IUsersRepository _usersRepository;
  private readonly IContractsRepository _contractsRepository;
  private readonly IWalletRepository _walletRepository;
  private readonly IJobsRepository _jobsRepository;
  private readonly IEscrowReleaseScheduler _escrowReleaseScheduler;
  private readonly IUnitOfWork _unitOfWork;

  public ApproveContractMilestoneCommandHandler(IUser user, IUsersRepository usersRepository, IContractsRepository contractsRepository, IWalletRepository walletRepository, IJobsRepository jobsRepository, IEscrowReleaseScheduler escrowReleaseScheduler, IUnitOfWork unitOfWork)
  {
    _user = user;
    _usersRepository = usersRepository;
    _contractsRepository = contractsRepository;
    _walletRepository = walletRepository;
    _jobsRepository = jobsRepository;
    _escrowReleaseScheduler = escrowReleaseScheduler;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result<Updated>> Handle(ApproveContractMilestoneCommand request, CancellationToken cancellationToken)
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

    var approveResult = contract.ApproveMilestone(request.MilestoneId);
    if (approveResult.IsError) return approveResult.Errors;

    var milestone = contract.ContractMilestones.First(m => m.Id == request.MilestoneId);

    var escrowTransactionResult = await _walletRepository.GetEscrowTransactionByContractMilestoneId(milestone.Id, cancellationToken);
    if (escrowTransactionResult.IsError) return escrowTransactionResult.Errors;
    var escrowTransaction = escrowTransactionResult.Value;

    var clientWalletResult = await _walletRepository.GetByUserIdAsync(contract.ClientId, cancellationToken);
    if (clientWalletResult.IsError) return clientWalletResult.Errors;
    var clientWallet = clientWalletResult.Value;

    var freelancerWalletResult = await _walletRepository.GetByUserIdAsync(contract.FreelancerId, cancellationToken);
    if (freelancerWalletResult.IsError) return freelancerWalletResult.Errors;
    var freelancerWallet = freelancerWalletResult.Value;

    var updatedWallet = clientWallet.AddTransaction(
      escrowTransaction.Amount,
      TransactionType.EscrowDeduction,
      WalletTransactionReferenceType.Contract,
      contract.Id,
      null,
      WalletTransactionStatus.Completed);
    if (updatedWallet.IsError) return updatedWallet.Errors;

    var platformFee = escrowTransaction.Amount * PlatformPolicy.PlatformFeePercentage;
    var feeEscrowTransaction = EscrowTransaction.Create(
      contract.Id,
      milestone.Id,
      EcrowTransactionType.PlatformFeeDeducted,
      platformFee,
      contract.ClientId,
      contract.FreelancerId,
      null);
    if (feeEscrowTransaction.IsError) return feeEscrowTransaction.Errors;
    _walletRepository.AddEscrowTransaction(feeEscrowTransaction.Value);

    freelancerWallet.AddTransaction(
      escrowTransaction.Amount - platformFee,
      TransactionType.EscrowAddition,
      WalletTransactionReferenceType.Contract,
      contract.Id,
      null,
      WalletTransactionStatus.Completed);

    var freelancerEscrowTransaction = EscrowTransaction.Create(
      contract.Id,
      milestone.Id,
      EcrowTransactionType.Hold,
      escrowTransaction.Amount - platformFee,
      contract.ClientId,
      contract.FreelancerId,
      null);
    if (freelancerEscrowTransaction.IsError) return freelancerEscrowTransaction.Errors;
    var freelancerTransaction = freelancerEscrowTransaction.Value;
    _walletRepository.AddEscrowTransaction(freelancerTransaction);

    _escrowReleaseScheduler.ScheduleEscrowRelease(freelancerTransaction.Id, PlatformPolicy.EscrowReleaseDelay);

    if (contract.ContractMilestones.All(m => m.Status == ContractMilestoneStatus.Approved))
    {
      var completeResult = contract.CompleteFromMilestones();
      if (completeResult.IsError) return completeResult.Errors;

      var jobResult = await _jobsRepository.GetJobByIdAsync(contract.JobId);
      if (jobResult.IsError) return jobResult.Errors;

      var job = jobResult.Value;
      job.UpdateStatus(JobStatus.Completed);
    }

    await _unitOfWork.SaveChangesAsync(cancellationToken);
    return Result.Updated;
  }
}
