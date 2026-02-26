using Kawadar.Application.Common.Interfaces.Repositories;
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

        public async Task<Result<Specilization>> GetByName(string name)
        {
            var specilization = await appDbContext.Specilizations.FirstOrDefaultAsync(s => s.Name == name);
            if (specilization is null) return Error.NotFound("Specilizaiton.NotFound", "Specilization not found");
            return specilization;
        }

        public async Task<Result<Specilization>> GetById(Guid Id)
        {
            var specilization = await appDbContext.Specilizations.FirstOrDefaultAsync(s => s.Id == Id);
            if (specilization is null) return Error.NotFound("Specilizaiton.NotFound", "Specilization not found");
            return specilization;
        }
    }
}
