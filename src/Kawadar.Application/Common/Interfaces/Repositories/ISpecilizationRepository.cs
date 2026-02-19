using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Specilizations;

namespace Kawadar.Application.Common.Interfaces.Repositories
{
    public interface ISpecilizationRepository
    {
        public Task<Result<Success>> AddAsync(Specilization Specilization);
        public Task<IEnumerable<Specilization>> GetAll(CancellationToken cancellationToken);
        public Result<Deleted> Delete(Specilization specilization);

        public Task<Result<Specilization>> GetByName(string name);
        public Task<Result<Specilization>> GetById(Guid Id);
    }
}
