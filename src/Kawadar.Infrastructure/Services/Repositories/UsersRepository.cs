using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Jobs.JobReports.Enums;
using Kawadar.Domain.UserProfiles;
using Kawadar.Domain.UserProfiles.Enums;
using Kawadar.Domain.UserProfiles.UserReports;
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
        var userProfile = await appDbContext.UserProfiles.Include(x => x.Reviews).FirstOrDefaultAsync(up => up.UserId == userId);

        if (userProfile == null) return Error.NotFound("UserProfile.NotFound", "User profile not found");

        return userProfile;

    }

    public async Task<Result<UserProfile>> GetUserProfileByIdAsync(Guid id)
    {
        var userProfile = await appDbContext.UserProfiles
        .Include(up => up.Specialization)
        .Include(up => up.Skills)
        .Include(up => up.Reviews)
        .FirstOrDefaultAsync(up => up.Id == id);

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
        var query = appDbContext.UserProfiles.Include(x => x.Reviews).Where(x => x.ProfileType != ProfileType.Admin).AsQueryable();
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

    public async Task<PaginatedList<UserProfile>> GetFreelancers(
        string? search,
        ExperienceYear? ExperienceYear,
        Guid? specilizationId,
        float? minumumRating,
        int page,
        int pageSize,
        string sortBy)
    {
        var query = appDbContext.UserProfiles.Include(x => x.Reviews).Where(x => x.ProfileType == ProfileType.Freelancer && x.IsBanned == false && x.IsDeleted == false).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Specialization.Name.Contains(search) || x.Title.Contains(search) || x.FullName.Contains(search));
        }

        if (ExperienceYear.HasValue)
        {
            query = query.Where(u => u.ExperienceYear == ExperienceYear);
        }

        if (specilizationId.HasValue)
        {
            query = query.Where(u => u.SpecializationId == specilizationId);
        }

        if (minumumRating.HasValue)
        {
            query = query.Where(u => u.Reviews.Select(x => x.Rating).DefaultIfEmpty(0).Average() >= minumumRating);
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
        foreach (var id in Ids)
        {
            var user = await appDbContext.UserProfiles
            .Include(up => up.Specialization)
            .Include(up => up.Skills)
            .Include(up => up.Reviews)
            .FirstOrDefaultAsync(x => x.Id == id);
            if (user is null) return Error.NotFound("User Profile not found");
            users.Add(user);
        }
        return users;
    }

    public async Task<Result<UsersRoleCount>> GetUsersRoleCount()
    {
        var TotalCount = await appDbContext.UserProfiles.Where(x => x.ProfileType != ProfileType.Admin).CountAsync();
        var FreelancersCount = await appDbContext.UserProfiles.Where(x => x.ProfileType == ProfileType.Freelancer).CountAsync();

        return new UsersRoleCount
        {
            TotalCount = TotalCount,
            FreelancersCount = FreelancersCount,
            ClientsCount = TotalCount - FreelancersCount
        };
    }

    public async Task<Result<int>> GetNewUsersThisMonth()
    {
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var nextMonthStart = monthStart.AddMonths(1);
        var newUsersCount = await appDbContext.UserProfiles.Where(x => x.CreatedAt >= monthStart && x.CreatedAt < nextMonthStart).CountAsync();
        return newUsersCount;
    }

    public async Task<int> GetVerifiedUserCount()
    {
        return await appDbContext.UserProfiles.Where(x => x.IsIdentityVerified == true).CountAsync();
    }

    public async Task<Success> AddUserReport(UserReport report)
    {
        await appDbContext.UserReports.AddAsync(report);
        return Result.Success;
    }

    public async Task<Result<UserReport>> GetUserReportById(Guid Id)
    {
        var report = await appDbContext.UserReports.Where(x => x.Id == Id).FirstOrDefaultAsync();
        if (report is null) return Error.NotFound("User Report not found");
        return report;
    }
    public async Task<PaginatedList<UserReport>> GetUserReports(ReportType? reportType, ReportStatus? reportStatus, int page, int pageSize, string sortBy)
    {
        var query = appDbContext.UserReports.AsQueryable();
        if (reportType.HasValue)
        {
            query = query.Where(x => x.ReportType == reportType);
        }

        if (reportStatus.HasValue)
        {
            query = query.Where(x => x.ReportStatus == reportStatus);
        }
        query = sortBy == "oldest"
            ? query.OrderBy(x => x.CreatedAt)
            : query.OrderByDescending(x => x.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();

        return new PaginatedList<UserReport>(items, totalCount, page, pageSize);
    }

    public async Task<PaginatedList<UserReport>> GetUserReportsByUserId(Guid Id, ReportStatus? reportstatus, ReportType? reportType, int page, int pageSize, string sortBy)
    {
        var query = appDbContext.UserReports.AsQueryable();
        query = query.Where(x => x.ReportedUser == Id);

        if (reportType.HasValue)
        {
            query = query.Where(x => x.ReportType == reportType);
        }

        if (reportstatus.HasValue)
        {
            query = query.Where(x => x.ReportStatus == reportstatus);
        }
        query = sortBy == "oldest"
            ? query.OrderBy(x => x.CreatedAt)
            : query.OrderByDescending(x => x.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();

        return new PaginatedList<UserReport>(items, totalCount, page, pageSize);
    }
}