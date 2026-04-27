
using Kawadar.Application.Common.Errors;
using Microsoft.Extensions.Logging;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Contracts;
using Kawadar.Domain.Contracts.Enums;
using Kawadar.Domain.Proposals.Enums;
using Kawadar.Domain.WalletAndPayments;
using Kawadar.Domain.WalletAndPayments.Enums;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Commands.CreateContract;


public class CreateContractCommandHandler : IRequestHandler<CreateContractCommand, Result<Guid>>
{

  private readonly IUser _user;
  private readonly IProposalsRepository _proposalsRepository;
  private readonly IUsersRepository _usersRepository;
  private readonly IUnitOfWork _unitOfWork;
  private readonly IJobsRepository _jobsRepository;
  private readonly IContractsRepository _contractsRepository;
  private readonly IWalletRepository _walletRepository;
  private readonly ILogger<CreateContractCommandHandler> _logger;

  public CreateContractCommandHandler(IUser user, IProposalsRepository proposalsRepository, IUsersRepository usersRepository, IUnitOfWork unitOfWork, IContractsRepository contractsRepository, IJobsRepository jobsRepository, IWalletRepository walletRepository, ILogger<CreateContractCommandHandler> logger)
  {
    _user = user;
    _proposalsRepository = proposalsRepository;
    _usersRepository = usersRepository;
    _unitOfWork = unitOfWork;
    _contractsRepository = contractsRepository;
    _jobsRepository = jobsRepository;
    _walletRepository = walletRepository;
    _logger = logger;
  }
  public async Task<Result<Guid>> Handle(CreateContractCommand request, CancellationToken cancellationToken)
  {
    var userId = _user.Id;
    if (userId == null) return ApplicationErrors.UserIsNotAuthenticated;

    // get user profile 
    var userProfileResult = await _usersRepository.GetUserProfileByUserIdAsync(userId);
    if (userProfileResult.IsError) return userProfileResult.Errors;
    var userProfile = userProfileResult.Value;
    // get job 
    var jobResult = await _jobsRepository.GetJobByIdAsync(request.JobId);
    if (jobResult.IsError) return jobResult.Errors;
    var job = jobResult.Value;
    // check if user is the client who posted the job
    if (job.PostedById != userProfile.Id) return ApplicationErrors.UnauthorizedAccess;
    // get the proposal that got accepted for the job
    var proposalResult = await _proposalsRepository.GetDetailsByIdAsync(request.ProposaslId);
    if (proposalResult.IsError) return proposalResult.Errors;
    var proposal = proposalResult.Value;
    // check if the proposal is for the same job
    if (proposal.JobId != request.JobId) return Error.Validation("Contracts.ProposalJobMismatch", "The proposal is not for the specified job.");
    // get the freelancer profile
    var freelancerProfileResult = await _usersRepository.GetUserProfileByIdAsync(proposal.FreelancerId);
    if (freelancerProfileResult.IsError) return freelancerProfileResult.Errors;
    var freelancerProfile = freelancerProfileResult.Value;




    // check that contact type matches the proposal type
    if (request.ContractType == ContractType.OneTime && proposal.ProposalType != JobProposalType.OneTime)
      return Error.Validation("Contracts.ContractProposalTypeMismatch", "The contract type does not match the proposal type.");
    if (request.ContractType == ContractType.Hourly && proposal.ProposalType != JobProposalType.Hourly)
      return Error.Validation("Contracts.ContractProposalTypeMismatch", "The contract type does not match the proposal type.");
    if (request.ContractType == ContractType.MilestoneBased && proposal.ProposalType != JobProposalType.MilestoneBased)
      return Error.Validation("Contracts.ContractProposalTypeMismatch", "The contract type does not match the proposal type.");



    // create contract 
    var contract = Contract.Create(request.JobId, request.ProposaslId, userProfile.Id, freelancerProfile.Id, request.ContractType, request.StartDate, request.ContractType == ContractType.OneTime ? DateTime.UtcNow.AddDays(Convert.ToDouble(proposal.EstimatedDays)) : null, request.ContractType == ContractType.OneTime ? proposal.Amount : null);

    if (contract.IsError) return contract.Errors;

    var createdContract = contract.Value;
    _contractsRepository.Add(createdContract);


    // start paying and ecrow process for the contract for one time contract
    if (request.ContractType == ContractType.OneTime && proposal.ProposalType == JobProposalType.OneTime)
    {
      // get client wallet  
      var clientWalletResult = await _walletRepository.GetByUserIdAsync(userProfile.Id, cancellationToken);
      if (clientWalletResult.IsError) return clientWalletResult.Errors;
      var clientWallet = clientWalletResult.Value;
      // reserve the amount in client's wallet
      var balanceBefore = clientWallet.Balance;
      var holdResult = clientWallet.Hold(proposal.Amount!.Value);
      if (holdResult.IsError) return holdResult.Errors;
      var balanceAfter = clientWallet.Balance;

      // make wallet transaction 
      var transactionResult = clientWallet.AddTransaction(proposal.Amount!.Value
      , balanceBefore, balanceAfter, TransactionType.EscrowHold, WalletTransactionReferenceType.Contract, createdContract.Id, null);
      if (transactionResult.IsError) return transactionResult.Errors;
      var transaction = transactionResult.Value;
      // mark transaction as completed
      transaction.MarkCompleted();

      // // explicitly add to DbContext so EF tracks it as Added (not Modified)
      // _walletRepository.AddWalletTransaction(transaction);

      // make escrow hold transaction 
      var escrowTansactionResult = EscrowTransaction.Create(createdContract.Id, null, EcrowTransactionType.Hold, proposal.Amount!.Value, userProfile.Id, freelancerProfile.Id, null);
      if (escrowTansactionResult.IsError) return escrowTansactionResult.Errors;
      var escrowTransaction = escrowTansactionResult.Value;
      _walletRepository.AddEscrowTransaction(escrowTransaction);

    }


    // for milestone based start created contractmilestones
    if (request.ContractType == ContractType.MilestoneBased && proposal.ProposalType == JobProposalType.MilestoneBased)
    {
      foreach (var milestone in proposal.Milestones)
      {
        createdContract.AddContractMilestone(milestone.Id, milestone.Title, milestone.Description, milestone.Amount, milestone.DueDate);
      }
    }


    // change status of the proposal to accepted
    proposal.UpdateState(JobProposalStatus.Accepted);
    job.UpdateStatus(Domain.Jobs.Enums.JobStatus.InProgress);

    try
    {
      await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    catch (Exception ex) when (ex.GetType().Name == "DbUpdateConcurrencyException")
    {
      _logger.LogError("CONCURRENCY ERROR: {Message} | InnerException: {Inner} | StackTrace: {Stack}",
        ex.Message, ex.InnerException?.Message ?? "none", ex.StackTrace);
      throw;
    }

    return createdContract.Id;
  }






}
