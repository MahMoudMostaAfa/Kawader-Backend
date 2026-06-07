using Kawadar.Application.Common.Interfaces;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles;

namespace kawadar.Application.SubcutaneousTests.Common.Fakes;

public class FakeFreelancerVectorStore : IFreelancerVectorStore
{
    public Task AddFreelancerAsync(UserProfile freelancer) => Task.CompletedTask;
    public Task UpdateFreelancerAsync(UserProfile freelancer) => Task.CompletedTask;
    public Task RemoveFreelancerAsync(Guid freelancerId) => Task.CompletedTask;

    public Task<Result<List<UserProfile>>> SearchFreelancersIdsAsync(string query, int topK)
        => Task.FromResult<Result<List<UserProfile>>>(new List<UserProfile>());
}
