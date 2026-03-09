using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Proposals;
using Kawadar.Infrastructure.Data;

namespace Kawadar.Infrastructure.Services.Repositories;

public class ProposalsRepository : IProposalsRepository
{
  private readonly AppDbContext _context;

  public ProposalsRepository(AppDbContext context)
  {
    _context = context;

  }
  public async Task AddAsync(JobProposal proposal, CancellationToken cancellationToken = default)
  {
    await _context.JobProposals.AddAsync(proposal, cancellationToken);
  }
}