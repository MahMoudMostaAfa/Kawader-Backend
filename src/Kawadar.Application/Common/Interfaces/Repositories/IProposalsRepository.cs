using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Proposals;

namespace Kawadar.Application.Common.Interfaces.Repositories;

public interface IProposalsRepository
{
  public Task AddAsync(JobProposal proposal, CancellationToken cancellationToken = default);

  public Task<Result<JobProposal>> GetDetailsByIdAsync(Guid proposalId, CancellationToken ct = default);
  public Task<Result<JobProposal>> GetByIdAsync(Guid proposalId, CancellationToken cancellationToken = default);
}