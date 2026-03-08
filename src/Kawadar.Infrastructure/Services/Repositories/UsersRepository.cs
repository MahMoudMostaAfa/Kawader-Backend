using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs;
using Kawadar.Domain.UserProfiles;
using Kawadar.Domain.UserProfiles.Enums;
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

    public async Task<PaginatedList<UserProfile>> GetUsers(
        bool? IsDeleted,
        bool? IsBanned,
        ExperienceYear? ExperienceYear,
        Guid? specilizationId,
        int page,
        int pageSize,
        string sortBy)
    {
        var query = appDbContext.UserProfiles.Where(x => x.ProfileType != ProfileType.Admin).AsQueryable();
        if (IsDeleted.HasValue)
        {
            query = query.Where(u => u.IsDeleted == IsDeleted);
        }

        if (IsBanned.HasValue)
        {
            query = query.Where(u => u.IsBanned == true);
        }

        if (ExperienceYear.HasValue)
        {
            query = query.Where(u => u.ExperienceYear == ExperienceYear);
        }

        if (specilizationId.HasValue)
        {
            query = query.Where(u => u.SpecializationId == specilizationId);
        }

        query = sortBy == "oldest"
            ? query.OrderBy(j => j.CreatedAt)
            : query.OrderByDescending(j => j.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();

        return new PaginatedList<UserProfile>(items, totalCount, page, pageSize);
    }

    public async Task<PaginatedList<UserProfile>> GetAdmins(
        bool? IsOnline,
        bool? IsDeleted,
        int page,
        int pageSize,
        string sortBy)
    {
        var query = appDbContext.UserProfiles.Where(x => x.ProfileType == ProfileType.Admin).AsQueryable();

        if (IsOnline.HasValue)
        {
            query = query.Where(a => a.IsOnline == IsOnline);
        }

        if (IsDeleted.HasValue)
        {
            query = query.Where(a => a.IsDeleted == IsDeleted);
        }

        query = sortBy == "oldest"
            ? query.OrderBy(j => j.CreatedAt)
            : query.OrderByDescending(j => j.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();

        return new PaginatedList<UserProfile>(items, totalCount, page, pageSize);
    }

    public async Task<Result<IEnumerable<UserProfile>>> GetUsersbyIds(IEnumerable<Guid> Ids)
    {
        List<UserProfile> users = new();
        foreach(var id in Ids)
        {
            var user = await appDbContext.UserProfiles.FirstOrDefaultAsync(x => x.Id == id);
            if (user is null) return Error.NotFound("User Profile not found");
            users.Add(user);
        }
        return users;
    }
}