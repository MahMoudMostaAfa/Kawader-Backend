using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Specilizations;

namespace kawadar.Application.SubcutaneousTests.Common.InMemory;

public class InMemorySpecilizationRepository : ISpecilizationRepository
{
    public readonly List<Specilization> Specilizations = [];

    public Task<Result<Success>> AddAsync(Specilization Specilization)
    {
        Specilizations.Add(Specilization);
        return Task.FromResult<Result<Success>>(Result.Success);
    }

    public Task<IEnumerable<Specilization>> GetAll(CancellationToken cancellationToken)
    {
        return Task.FromResult(Specilizations.AsEnumerable());
    }

    public Result<Deleted> Delete(Specilization specilization)
    {
        Specilizations.Remove(specilization);
        return Result.Deleted;
    }

    public Task<Result<Specilization>> GetByName(string name)
    {
        var spec = Specilizations.FirstOrDefault(s => s.Name == name);
        return Task.FromResult(spec is not null
            ? (Result<Specilization>)spec
            : Error.NotFound("Specilization.NotFound", $"Specilization '{name}' not found."));
    }

    public Task<Result<Specilization>> GetById(Guid Id)
    {
        var spec = Specilizations.FirstOrDefault(s => s.Id == Id);
        return Task.FromResult(spec is not null
            ? (Result<Specilization>)spec
            : Error.NotFound("Specilization.NotFound", $"Specilization '{Id}' not found."));
    }

    public void Clear() => Specilizations.Clear();
}
