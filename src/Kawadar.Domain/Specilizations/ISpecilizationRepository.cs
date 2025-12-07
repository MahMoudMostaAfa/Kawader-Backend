
using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Specilizations
{
    public interface ISpecilizationRepository
    {
        public Task<Result<Success>> AddAsync(Specilization Specilization);
        public Task<IEnumerable<Specilization>> GetAll(CancellationToken cancellationToken);
        public Result<Deleted> Delete(Specilization specilization);
        public Task<Result<Updated>> Update(Guid Id, Specilization UpdatedSpecilization);
    }
}
