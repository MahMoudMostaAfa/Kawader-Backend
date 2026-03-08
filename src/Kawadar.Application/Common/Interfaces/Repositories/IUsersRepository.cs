using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles;
using Kawadar.Domain.UserProfiles.Enums;

namespace Kawadar.Application.Common.Interfaces.Repositories;

public interface IUsersRepository
{

  Task<Result<Success>> CreateUserProfileAsync(UserProfile userProfile);

  Task<Result<UserProfile>> GetUserProfileByUserIdAsync(string userId);

  Task<Result<UserProfile>> GetUserProfileByIdAsync(Guid id);

    Task<PaginatedList<UserProfile>> GetUsers(
        bool? IsDeleted,
        bool? IsBanned,
        ExperienceYear? ExperienceYear,
        Guid? specilizationId,
        int page,
        int pageSize,
        string sortBy);
    Task<PaginatedList<UserProfile>> GetAdmins(
        bool? IsOnline,
        bool? IsDeleted,
        int page,
        int pageSize,
        string sortBy);
    Task<Result<IEnumerable<UserProfile>>> GetUsersbyIds(IEnumerable<Guid> Ids);
}