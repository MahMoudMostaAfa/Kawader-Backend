using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Proposals;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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

  public async Task<Result<JobProposal>> GetByIdAsync(Guid proposalId, CancellationToken cancellationToken = default)
  {
    var proposal = await _context.JobProposals.Include(p => p.QuestionAnswers)
      .Include(p => p.Milestones)
    .FirstOrDefaultAsync(p => p.Id == proposalId, cancellationToken);
    if (proposal is null) return Error.NotFound(description: "Proposal not found");

    return proposal;
  }

  public async Task<Result<JobProposal>> GetDetailsByIdAsync(Guid proposalId, CancellationToken ct = default)
  {
    var proposal = await _context.JobProposals.Include(p => p.Milestones).Include(p => p.QuestionAnswers)
    .ThenInclude(qa => qa.Question)
    .FirstOrDefaultAsync(p => p.Id == proposalId, ct);

    if (proposal is null) return Error.NotFound(description: "Proposal not found");

    return proposal;

  }
}