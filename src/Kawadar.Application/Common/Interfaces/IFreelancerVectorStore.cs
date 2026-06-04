using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles;

namespace Kawadar.Application.Common.Interfaces;

public interface IFreelancerVectorStore
{
  Task AddFreelancerAsync(UserProfile freelancer);
  Task UpdateFreelancerAsync(UserProfile freelancer);
  Task RemoveFreelancerAsync(Guid freelancerId);
  Task<Result<List<UserProfile>>> SearchFreelancersIdsAsync(string query, int topK);
}