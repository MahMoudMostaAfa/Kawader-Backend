using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Violations;
using Kawadar.Domain.Violations.Enums;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kawadar.Infrastructure.Services.Repositories
{
    public class ViolationRepository(AppDbContext appDbContext) : IViolationRepository
    {
        public async Task<Result<Success>> AddViolation(Violation violation)
        {
            await appDbContext.Violations.AddAsync(violation);
            return Result.Success;
        }

        public async Task<PaginatedList<Violation>> GetAllViolation(ViolationStatus? status, ViolationType? type, int page, int pageSize, string sortBy)
        {
            var query = appDbContext.Violations.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(x => x.ViolationStatus == status);
            }

            if (type.HasValue)
            {
                query = query.Where(x => x.ViolationType == type);
            }

            query = sortBy == "oldest"
                ? query.OrderBy(j => j.CreatedAt)
                : query.OrderByDescending(j => j.CreatedAt);

            var totalCount = await query.CountAsync();

            var items = await query
              .Skip((page - 1) * pageSize)
              .Take(pageSize)
              .ToListAsync();

            return new PaginatedList<Violation>(items, totalCount, page, pageSize);
        }

        public async Task<Result<Violation>> GetViolationById(Guid Id)
        {
            var violation = await appDbContext.Violations.Where(x => x.Id == Id).FirstOrDefaultAsync();
            if (violation is null) return Error.NotFound("This Violation doesn't exist");
            return violation;
        }
    }
}
