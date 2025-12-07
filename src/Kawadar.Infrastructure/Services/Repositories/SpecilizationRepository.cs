using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Specilizations;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kawadar.Infrastructure.Services.Repositories
{
    public class SpecilizationRepository(AppDbContext appDbContext) : ISpecilizationRepository
    {
        public async Task<Result<Success>> AddAsync(Specilization Specilization)
        {
            await appDbContext.Specilizations.AddAsync(Specilization);

            return Result.Success;
        }

        public Result<Deleted> Delete(Specilization specilization)
        {
            appDbContext.Specilizations.Remove(specilization);
            return Result.Deleted;
        }

        public async Task<IEnumerable<Specilization>> GetAll(CancellationToken cancellationToken)
        {
            var specilizations = await appDbContext.Specilizations.ToListAsync(cancellationToken);
            return specilizations;
        }

        public async Task<Result<Updated>> Update(Guid Id, Specilization UpdatedSpecilization)
        {
            var specilization = await appDbContext.Specilizations.FirstOrDefaultAsync();
            specilization.Update(UpdatedSpecilization.Name, UpdatedSpecilization.IsActive);
            return Result.Updated;
        }
    }
}
