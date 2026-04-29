using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Contracts.Disbutes;
using Kawadar.Domain.Contracts.Disbutes.Enum;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kawadar.Infrastructure.Services.Repositories
{
    public class DisbuteRepository(AppDbContext context) : IDisbuteRepository
    {
        public async Task<Result<Success>> AddDisbute(Disbute disbute)
        {
            await context.Disbutes.AddAsync(disbute);
            return Result.Success;
        }

        public async Task<PaginatedList<Disbute>> GetAllDisbutes(DisbuteStatus? status, int page, int pageSize, string sortBy)
        {
            var query = context.Disbutes.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status);
            }

            query = sortBy == "oldest" ? query.OrderBy(x => x.CreatedAt) : query.OrderByDescending(x => x.CreatedAt);
            var totalCount = await query.CountAsync();

            var items = await query.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<Disbute>(items, totalCount, page, pageSize);
        }

        public async Task<Result<Disbute>> GetDisbuteById(Guid Id)
        {
            var disbute = await context.Disbutes.Where(x => x.Id == Id).FirstOrDefaultAsync();
            if (disbute is null) return Error.NotFound("This disbute doesn't exist");
            return disbute;
        }
    }
}
