using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Proposals;
using Kawadar.Domain.Proposals.Enums;
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

  public async Task<Result<PaginatedList<JobProposal>>> GetFreelancerProposals(Guid FreelancerId, int Page = 1,
  int PageSize = 10, string SortBy = "newest")
  {
    var query = _context.JobProposals.Where(jp => jp.FreelancerId == FreelancerId).AsQueryable();
    var orderedQuery = string.Equals(SortBy, "oldest", StringComparison.OrdinalIgnoreCase)
    ? query.OrderBy(jp => jp.CreatedAt)
    : query.OrderByDescending(jp => jp.CreatedAt);

    var totalCount = await query.CountAsync();
    var items = await orderedQuery
      .Skip((Page - 1) * PageSize)
      .Take(PageSize)
      .ToListAsync();

    return new PaginatedList<JobProposal>(items, totalCount, Page, PageSize);

  }

  public async Task<Result<PaginatedList<JobProposal>>> GetProposalsAsync(Guid jobId, JobProposalType? Type, JobProposalStatus? Status, int Page = 1, int PageSize = 10, string DatesortBy = "newest", string? PriceSortBy = null, string? EstimatedTimeSortBy = null)
  {
    var query = _context.JobProposals.Where(jp => jp.JobId == jobId).AsQueryable();

    if (Type.HasValue) query = query.Where(jp => jp.ProposalType == Type.Value);

    if (Status.HasValue) query = query.Where(jp => jp.Status == Status.Value && jp.Status != JobProposalStatus.Withdrawn);

    var orderedQuery = string.Equals(DatesortBy, "oldest", StringComparison.OrdinalIgnoreCase)
      ? query.OrderBy(jp => jp.CreatedAt)
      : query.OrderByDescending(jp => jp.CreatedAt);


    if (!string.IsNullOrEmpty(PriceSortBy))
    {
      orderedQuery = string.Equals(PriceSortBy, "lowest", StringComparison.OrdinalIgnoreCase)
        ? orderedQuery.ThenBy(jp =>
          jp.ProposalType == JobProposalType.MilestoneBased
            ? (jp.Milestones.Sum(m => (decimal?)m.Amount) ?? 0m)
            : (jp.Amount ?? (jp.HourlyRate * (jp.EstimatedHours ?? 0m)) ?? 0m))
        : orderedQuery.ThenByDescending(jp =>
          jp.ProposalType == JobProposalType.MilestoneBased
            ? (jp.Milestones.Sum(m => (decimal?)m.Amount) ?? 0m)
            : (jp.Amount ?? (jp.HourlyRate * (jp.EstimatedHours ?? 0m)) ?? 0m));
    }
    if (!string.IsNullOrEmpty(EstimatedTimeSortBy))
    {
      orderedQuery = string.Equals(EstimatedTimeSortBy, "earliest", StringComparison.OrdinalIgnoreCase)
        || string.Equals(EstimatedTimeSortBy, "earlist", StringComparison.OrdinalIgnoreCase)
        ? orderedQuery.ThenBy(jp => jp.EstimatedDays ?? jp.EstimatedHours ?? 0m)
        : orderedQuery.ThenByDescending(jp => jp.EstimatedDays ?? jp.EstimatedHours ?? 0m);
    }

    var totalCount = await query.CountAsync();
    var items = await orderedQuery
      .Skip((Page - 1) * PageSize)
      .Take(PageSize)
      .ToListAsync();

    return new PaginatedList<JobProposal>(items, totalCount, Page, PageSize);
  }
    
    public async Task<int> GetProposalsCount()
    {
        return await _context.JobProposals.CountAsync();
    }

    public async Task<Result<int>> GetUserProposalsThisMonth(Guid UserProfileId)
    {
        var userProfile = await _context.UserProfiles.Where(x => x.Id == UserProfileId).FirstOrDefaultAsync();
        if (userProfile is null) return Error.NotFound();

        var userJoinedAt = userProfile.CreatedAt;
        var now = DateTime.UtcNow;
        var monthsSinceJoin = ((now.Year - userJoinedAt.Year) * 12) + now.Month - userJoinedAt.Month;
        var currentCycleStart = userJoinedAt.AddMonths(monthsSinceJoin);
        var currentCycleEnd = currentCycleStart.AddMonths(1);

        var currentProposalsCount = await _context.JobProposals
            .Where(x => x.FreelancerId == UserProfileId
                && x.CreatedAt >= currentCycleStart
                && x.CreatedAt < currentCycleEnd) 
            .CountAsync();

        return currentProposalsCount;
    }

    public async Task<int> GetNumberOfProposalsThisMonth()
    {
        var now = DateTime.UtcNow;
        var currentMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var NextMonthStart = currentMonthStart.AddMonths(1);

        var proposals = await _context.JobProposals.Where(x => x.CreatedAt >= currentMonthStart && x.CreatedAt < NextMonthStart).CountAsync();
        return proposals;
    }

    public async Task<Dictionary<JobProposalStatus, int>> GetDistributionBasedOnStatus()
    {
        var distribution = await _context.JobProposals.GroupBy(x => x.Status).ToDictionaryAsync(x => x.Key, x => x.Count());
        return distribution;
    }

}