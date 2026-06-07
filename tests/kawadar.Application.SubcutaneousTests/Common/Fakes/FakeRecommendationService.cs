using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;

namespace kawadar.Application.SubcutaneousTests.Common.Fakes;

public class FakeRecommendationService : IRecommendationService
{
    public Task<Result<Success>> InsertUserAsync(Guid userId, object? labels = null, string? comment = null, CancellationToken ct = default)
        => Task.FromResult<Result<Success>>(Result.Success);

    public Task<Result<Success>> InsertUsersAsync(IEnumerable<RecommendationUser> users, CancellationToken ct = default)
        => Task.FromResult<Result<Success>>(Result.Success);

    public Task<Result<Success>> UpdateUserAsync(Guid userId, object? labels = null, string? comment = null, CancellationToken ct = default)
        => Task.FromResult<Result<Success>>(Result.Success);

    public Task<Result<RecommendationUser>> GetUserAsync(Guid userId, CancellationToken ct = default)
        => Task.FromResult<Result<RecommendationUser>>(new RecommendationUser(userId));

    public Task<Result<Deleted>> DeleteUserAsync(Guid userId, CancellationToken ct = default)
        => Task.FromResult<Result<Deleted>>(Result.Deleted);

    public Task<Result<Success>> InsertItemAsync(string itemId, string[]? categories = null, object? labels = null, string? comment = null, CancellationToken ct = default)
        => Task.FromResult<Result<Success>>(Result.Success);

    public Task<Result<Success>> InsertItemsAsync(IEnumerable<RecommendationItem> items, CancellationToken ct = default)
        => Task.FromResult<Result<Success>>(Result.Success);

    public Task<Result<RecommendationItem>> GetItemAsync(string itemId, CancellationToken ct = default)
        => Task.FromResult<Result<RecommendationItem>>(new RecommendationItem(itemId));

    public Task<Result<Deleted>> DeleteItemAsync(string itemId, CancellationToken ct = default)
        => Task.FromResult<Result<Deleted>>(Result.Deleted);

    public Task<Result<Success>> InsertFeedbackAsync(IEnumerable<RecommendationFeedback> feedbacks, CancellationToken ct = default)
        => Task.FromResult<Result<Success>>(Result.Success);

    public Task<Result<PaginatedList<Guid>>> GetRecommendationsAsync(Guid userId, int pageNumber, int pageSize, CancellationToken ct = default)
        => Task.FromResult<Result<PaginatedList<Guid>>>(
            new PaginatedList<Guid>(new List<Guid>(), 0, pageNumber, pageSize));

    public Task<Result<string[]>> GetRecommendationsRawAsync(Guid userId, CancellationToken ct = default)
        => Task.FromResult<Result<string[]>>(Array.Empty<string>());

    public Task<Result<List<ScoredItem>>> GetUserNeighborsAsync(Guid userId, int count = 10, CancellationToken ct = default)
        => Task.FromResult<Result<List<ScoredItem>>>(new List<ScoredItem>());

    public Task<Result<Success>> ResetAsync(CancellationToken ct = default)
        => Task.FromResult<Result<Success>>(Result.Success);
}
