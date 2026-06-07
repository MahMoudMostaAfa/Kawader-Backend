using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Proposals;
using Kawadar.Domain.Proposals.Enums;

namespace kawadar.Application.SubcutaneousTests.Common.InMemory;

public class InMemoryProposalsRepository : IProposalsRepository
{
    public readonly List<JobProposal> Proposals = [];

    public Task AddAsync(JobProposal proposal, CancellationToken cancellationToken = default)
    {
        Proposals.Add(proposal);
        return Task.CompletedTask;
    }

    public Task<Result<JobProposal>> GetDetailsByIdAsync(Guid proposalId, CancellationToken ct = default)
    {
        var proposal = Proposals.FirstOrDefault(p => p.Id == proposalId);
        return Task.FromResult(proposal is not null
            ? (Result<JobProposal>)proposal
            : Error.NotFound("Proposal.NotFound", $"Proposal '{proposalId}' not found."));
    }

    public Task<Result<JobProposal>> GetByIdAsync(Guid proposalId, CancellationToken cancellationToken = default)
    {
        var proposal = Proposals.FirstOrDefault(p => p.Id == proposalId);
        return Task.FromResult(proposal is not null
            ? (Result<JobProposal>)proposal
            : Error.NotFound("Proposal.NotFound", $"Proposal '{proposalId}' not found."));
    }

    public Task<Result<bool>> ProposalExistsForJobAndFreelancerAsync(Guid jobId, Guid freelancerId, CancellationToken cancellationToken = default)
    {
        var exists = Proposals.Any(p => p.JobId == jobId && p.FreelancerId == freelancerId);
        return Task.FromResult<Result<bool>>(exists);
    }

    public Task<Result<PaginatedList<JobProposal>>> GetProposalsAsync(
        Guid jobId,
        JobProposalType? Type,
        JobProposalStatus? Status,
        int Page = 1,
        int PageSize = 10,
        string DatesortBy = "newest",
        string? PriceSortBy = null,
        string? EstimatedTimeSortBy = null)
    {
        var query = Proposals.Where(p => p.JobId == jobId);

        if (Type.HasValue) query = query.Where(p => p.ProposalType == Type.Value);
        if (Status.HasValue) query = query.Where(p => p.Status == Status.Value);

        var ordered = DatesortBy?.ToLowerInvariant() switch
        {
            "oldest" => query.OrderBy(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var all = ordered.ToList();
        var items = all.Skip((Page - 1) * PageSize).Take(PageSize).ToList();
        var paginated = new PaginatedList<JobProposal>(items, all.Count, Page, PageSize);
        return Task.FromResult<Result<PaginatedList<JobProposal>>>(paginated);
    }

    public Task<Result<PaginatedList<JobProposal>>> GetFreelancerProposals(
        Guid FreelancerId,
        int Page = 1,
        int PageSize = 10,
        string SortBy = "newest")
    {
        var query = Proposals.Where(p => p.FreelancerId == FreelancerId);

        var ordered = SortBy?.ToLowerInvariant() switch
        {
            "oldest" => query.OrderBy(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var all = ordered.ToList();
        var items = all.Skip((Page - 1) * PageSize).Take(PageSize).ToList();
        var paginated = new PaginatedList<JobProposal>(items, all.Count, Page, PageSize);
        return Task.FromResult<Result<PaginatedList<JobProposal>>>(paginated);
    }

    public Task<Dictionary<JobProposalStatus, int>> GetDistributionBasedOnStatus()
    {
        var distribution = Proposals
            .GroupBy(p => p.Status)
            .ToDictionary(g => g.Key, g => g.Count());
        return Task.FromResult(distribution);
    }

    public Task<int> GetNumberOfProposalsThisMonth()
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var count = Proposals.Count(p => p.CreatedAt >= startOfMonth);
        return Task.FromResult(count);
    }

    public Task<int> GetProposalsCount()
    {
        return Task.FromResult(Proposals.Count);
    }

    public void Clear() => Proposals.Clear();

    public Task<Result<int>> GetUserProposalsThisMonth(Guid UserProfileId)
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var count = Proposals.Count(p => p.FreelancerId == UserProfileId && p.CreatedAt >= startOfMonth);
        return Task.FromResult<Result<int>>(count);
    }
}
