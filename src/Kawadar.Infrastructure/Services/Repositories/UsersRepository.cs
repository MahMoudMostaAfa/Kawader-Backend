using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kawadar.Infrastructure.Services.Repositories;

public class UsersRepository(AppDbContext appDbContext) : IUsersRepository
{
  public async Task<Result<Success>> CreateUserProfileAsync(UserProfile userProfile)
  {

    await appDbContext.UserProfiles.AddAsync(userProfile);

    return Result.Success;
  }

  public async Task<Result<UserProfile>> GetUserProfileByUserIdAsync(string userId)
  {
    var userProfile = await appDbContext.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);

    if (userProfile == null) return Error.NotFound("UserProfile.NotFound", "User profile not found");

    return userProfile;

  }

  public async Task<Result<UserProfile>> GetUserProfileByIdAsync(Guid id)
  {
    var userProfile = await appDbContext.UserProfiles.FirstOrDefaultAsync(up => up.Id == id);

    if (userProfile == null) return Error.NotFound("UserProfile.NotFound", "User profile not found");

    return userProfile;
  }
}