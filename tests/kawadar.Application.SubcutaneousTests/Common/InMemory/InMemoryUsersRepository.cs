using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles;
using Kawadar.Domain.UserProfiles.Enums;
using Kawadar.Domain.UserProfiles.UserReports;
using Kawadar.Domain.Jobs.JobReports.Enums;

namespace kawadar.Application.SubcutaneousTests.Common.InMemory;

public class InMemoryUsersRepository : IUsersRepository
{
    public readonly List<UserProfile> Users = [];
    public readonly List<UserReport> UserReports = [];

    public Task<Result<Success>> CreateUserProfileAsync(UserProfile userProfile)
    {
        Users.Add(userProfile);
        return Task.FromResult<Result<Success>>(Result.Success);
    }

    public Task<Result<UserProfile>> GetUserProfileByUserIdAsync(string userId)
    {
        var user = Users.FirstOrDefault(u => u.UserId == userId);
        return Task.FromResult(user is not null
            ? (Result<UserProfile>)user
            : Error.NotFound("UserProfile.NotFound", $"UserProfile for userId '{userId}' not found."));
    }

    public Task<Result<UserProfile>> GetUserProfileByIdAsync(Guid id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(user is not null
            ? (Result<UserProfile>)user
            : Error.NotFound("UserProfile.NotFound", $"UserProfile '{id}' not found."));
    }

    public Task<PaginatedList<UserProfile>> GetUsers(
        bool? IsDeleted,
        bool? IsBanned,
        ExperienceYear? ExperienceYear,
        Guid? specilizationId,
        int page,
        int pageSize,
        string sortBy)
    {
        var query = Users.AsEnumerable();

        if (IsDeleted.HasValue) query = query.Where(u => u.IsDeleted == IsDeleted.Value);
        if (IsBanned.HasValue) query = query.Where(u => u.IsBanned == IsBanned.Value);
        if (ExperienceYear.HasValue) query = query.Where(u => u.ExperienceYear == ExperienceYear.Value);
        if (specilizationId.HasValue) query = query.Where(u => u.SpecializationId == specilizationId.Value);

        var all = query.ToList();
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PaginatedList<UserProfile>(items, all.Count, page, pageSize));
    }

    public Task<PaginatedList<UserProfile>> GetAdmins(
        bool? IsOnline,
        bool? IsDeleted,
        int page,
        int pageSize,
        string sortBy)
    {
        var query = Users.Where(u => u.ProfileType == ProfileType.Admin);

        if (IsOnline.HasValue) query = query.Where(u => u.IsOnline == IsOnline.Value);
        if (IsDeleted.HasValue) query = query.Where(u => u.IsDeleted == IsDeleted.Value);

        var all = query.ToList();
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PaginatedList<UserProfile>(items, all.Count, page, pageSize));
    }

    public async Task<Result<IEnumerable<UserProfile>>> GetUsersbyIds(IEnumerable<Guid> Ids)
    {
        await Task.CompletedTask;
        var idSet = Ids.ToHashSet();
        var found = Users.Where(u => idSet.Contains(u.Id)).ToList();
        return found;
    }

    public Task<Result<UsersRoleCount>> GetUsersRoleCount()
    {
        var count = new UsersRoleCount
        {
            TotalCount = Users.Count,
            FreelancersCount = Users.Count(u => u.ProfileType == ProfileType.Freelancer),
            ClientsCount = Users.Count(u => u.ProfileType == ProfileType.Client)
        };
        return Task.FromResult<Result<UsersRoleCount>>(count);
    }

    public Task<Result<int>> GetNewUsersThisMonth()
    {
        var count = Users.Count(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-30));
        return Task.FromResult<Result<int>>(count);
    }

    public Task<int> GetVerifiedUserCount()
    {
        return Task.FromResult(Users.Count(u => u.IsIdentityVerified == true));
    }

    public Task<Success> AddUserReport(UserReport report)
    {
        UserReports.Add(report);
        return Task.FromResult(Result.Success);
    }

    public Task<PaginatedList<UserReport>> GetUserReports(ReportType? reportType, ReportStatus? reportStatus, int page, int pageSize, string sortBy)
    {
        var query = UserReports.AsEnumerable();
        if (reportType.HasValue) query = query.Where(r => r.ReportType == reportType.Value);
        if (reportStatus.HasValue) query = query.Where(r => r.ReportStatus == reportStatus.Value);

        var all = query.ToList();
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PaginatedList<UserReport>(items, all.Count, page, pageSize));
    }

    public Task<PaginatedList<UserReport>> GetUserReportsByUserId(Guid Id, ReportStatus? reportstatus, ReportType? reportType, int page, int pageSize, string sortBy)
    {
        var query = UserReports.Where(r => r.ReportedUser == Id);
        if (reportType.HasValue) query = query.Where(r => r.ReportType == reportType.Value);
        if (reportstatus.HasValue) query = query.Where(r => r.ReportStatus == reportstatus.Value);

        var all = query.ToList();
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PaginatedList<UserReport>(items, all.Count, page, pageSize));
    }

    public Task<Result<UserReport>> GetUserReportById(Guid Id)
    {
        var report = UserReports.FirstOrDefault(r => r.Id == Id);
        return Task.FromResult(report is not null
            ? (Result<UserReport>)report
            : Error.NotFound("UserReport.NotFound", $"UserReport '{Id}' not found."));
    }

    public void Clear()
    {
        Users.Clear();
        UserReports.Clear();
    }
}
