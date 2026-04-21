using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobReports.Enums;
using Kawadar.Domain.UserProfiles;
using Kawadar.Domain.UserProfiles.Enums;
using Kawadar.Domain.UserProfiles.UserReports;

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
    Task<Result<UsersRoleCount>> GetUsersRoleCount();
    Task<Result<int>> GetNewUsersThisMonth();
    Task<int> GetVerifiedUserCount();
    Task<Success> AddUserReport(UserReport report);
    Task<PaginatedList<UserReport>> GetUserReports(ReportType? reportType, ReportStatus? reportStatus, int page, int pageSize, string sortBy);
    Task<PaginatedList<UserReport>> GetUserReportsByUserId(Guid Id, ReportStatus? reportstatus, ReportType? reportType, int page, int pageSize, string sortBy);
    Task<Result<UserReport>> GetUserReportById(Guid Id);
}