using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Proposals;
using Kawadar.Domain.Proposals.Enums;

namespace Kawadar.Application.Common.Interfaces.Repositories;

public interface IProposalsRepository
{
  public Task AddAsync(JobProposal proposal, CancellationToken cancellationToken = default);

  public Task<Result<JobProposal>> GetDetailsByIdAsync(Guid proposalId, CancellationToken ct = default);
  public Task<Result<JobProposal>> GetByIdAsync(Guid proposalId, CancellationToken cancellationToken = default);

  public Task<Result<bool>> ProposalExistsForJobAndFreelancerAsync(Guid jobId, Guid freelancerId, CancellationToken cancellationToken = default);


  public Task<Result<PaginatedList<JobProposal>>> GetProposalsAsync(
  Guid jobId,
   JobProposalType? Type,
    JobProposalStatus? Status,
    int Page = 1,
    int PageSize = 10,
    string DatesortBy = "newest",
    string? PriceSortBy = null,
    string? EstimatedTimeSortBy = null

  );
  public Task<Result<PaginatedList<JobProposal>>> GetFreelancerProposals(
  Guid FreelancerId,
   int Page = 1,
  int PageSize = 10,
  string SortBy = "newest"

  );

  public Task<Dictionary<JobProposalStatus, int>> GetDistributionBasedOnStatus();
  public Task<int> GetNumberOfProposalsThisMonth();
  public Task<int> GetProposalsCount();


}