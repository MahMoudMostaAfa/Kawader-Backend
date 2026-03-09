using Kawadar.Domain.Proposals;

namespace Kawadar.Application.Common.Interfaces.Repositories;

public interface IProposalsRepository
{
  public Task AddAsync(JobProposal proposal, CancellationToken cancellationToken = default);
}