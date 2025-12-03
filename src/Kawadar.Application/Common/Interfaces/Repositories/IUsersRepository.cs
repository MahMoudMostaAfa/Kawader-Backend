using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles;

namespace Kawadar.Application.Common.Interfaces.Repositories;

public interface IUsersRepository
{

  Task<Result<Success>> CreateUserProfileAsync(UserProfile userProfile);

  Task<Result<UserProfile>> GetUserProfileByUserIdAsync(string userId);
}