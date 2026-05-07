using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.BackgroundJobs;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.Enums;
using Kawadar.Domain.WalletAndPayments;
using Kawadar.Domain.WalletAndPayments.Enums;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Commands.AcceptContractCompletion;

public class AcceptContractCompletionCommandHandler :
IRequestHandler<AcceptContractCompletionCommand, Result<Updated>>
{
  private readonly IUser _user;
  private readonly IUnitOfWork _unitOfWork;

  private readonly IContractsRepository _contractsRepository;
  private readonly IUsersRepository _usersRepository;
  private readonly IWalletRepository _walletRepository;
  private readonly IJobsRepository _jobsRepository;
  private readonly IEscrowReleaseScheduler _escrowReleaseScheduler;

  public AcceptContractCompletionCommandHandler(IUser user, IUnitOfWork unitOfWork, IContractsRepository contractsRepository, IUsersRepository usersRepository, IWalletRepository walletRepository, IJobsRepository jobsRepository, IEscrowReleaseScheduler escrowReleaseScheduler)
  {
    _user = user;
    _unitOfWork = unitOfWork;
    _contractsRepository = contractsRepository;
    _usersRepository = usersRepository;
    _walletRepository = walletRepository;
    _jobsRepository = jobsRepository;
    _escrowReleaseScheduler = escrowReleaseScheduler;

  }

  public async Task<Result<Updated>> Handle(AcceptContractCompletionCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;

    if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;

    var userProfile = userProfileResult.Value;

    var contractResult = await _contractsRepository.GetContractByIdAsync(request.ContractId);
    if (contractResult.IsError) return contractResult.Errors;

    var contract = contractResult.Value;
    if (contract.ClientId != userProfile.Id)
      return ApplicationErrors.UnauthorizedAccess;

    var acceptCompletionResult = contract.ApproveCompletion();
    if (acceptCompletionResult.IsError) return acceptCompletionResult.Errors;


    var escrowTransactionResult = await _walletRepository.GetEscrowTransactionByContractId(contract.Id);
    if (escrowTransactionResult.IsError) return escrowTransactionResult.Errors;
    var escrowTransaction = escrowTransactionResult.Value;

    var freelancerWalletResult = await _walletRepository.GetByUserIdAsync(contract.FreelancerId);
    if (freelancerWalletResult.IsError) return freelancerWalletResult.Errors;
    var freelancerWallet = freelancerWalletResult.Value;

    var clientWalletResult = await _walletRepository.GetByUserIdAsync(contract.ClientId);
    if (clientWalletResult.IsError) return clientWalletResult.Errors;
    var clientWallet = clientWalletResult.Value;

    // deduct ecsrow ammount from client wallet
    var updatedWallet = clientWallet.AddTransaction(escrowTransaction.Amount, TransactionType.EscrowDeduction, WalletTransactionReferenceType.Contract, contract.Id, null, WalletTransactionStatus.Completed);
    if (updatedWallet.IsError) return updatedWallet.Errors;


    //  deduct platform fee from the ecrow transaction  and ecrow transaction for the freelancer
    var platformFee = escrowTransaction.Amount * PlatformPolicy.PlatformFeePercentage;

    var feeEscrowTransaction = EscrowTransaction.Create(contract.Id, null, EcrowTransactionType.PlatformFeeDeducted, platformFee, contract.ClientId, contract.FreelancerId, null);

    if (feeEscrowTransaction.IsError) return feeEscrowTransaction.Errors;
    var feeTransaction = feeEscrowTransaction.Value;

    _walletRepository.AddEscrowTransaction(feeTransaction);

    // add ecrow ammount to freelancer wallet (after deducting platform fee) and add transaction for the freelancer
    freelancerWallet.AddTransaction(escrowTransaction.Amount - platformFee, TransactionType.EscrowAddition, WalletTransactionReferenceType.Contract, contract.Id, null, WalletTransactionStatus.Completed);
    // add ecrow transaction for the freelancer
    var freelancerEscrowTransaction = EscrowTransaction.Create(contract.Id, null, EcrowTransactionType.Hold, escrowTransaction.Amount - platformFee, contract.ClientId, contract.FreelancerId, null);
    if (freelancerEscrowTransaction.IsError) return freelancerEscrowTransaction.Errors;
    var freelancerTransaction = freelancerEscrowTransaction.Value;
    _walletRepository.AddEscrowTransaction(freelancerTransaction);


    // mark  job as completed and schedule job for escrow release after 7 days (in case of no disputes)
    var jobResult = await _jobsRepository.GetJobByIdAsync(contract.JobId);
    if (jobResult.IsError) return jobResult.Errors;
    var job = jobResult.Value;

    job.UpdateStatus(JobStatus.Completed);


    _escrowReleaseScheduler.ScheduleEscrowRelease(freelancerTransaction.Id, TimeSpan.FromMinutes(2));

    await _unitOfWork.SaveChangesAsync(cancellationToken);
    return Result.Updated;
  }
}