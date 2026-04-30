using Kawadar.Application.Common.Interfaces.Repositories;

namespace kawadar.Application.SubcutaneousTests.Common.InMemory;

public class InMemoryUnitOfWork : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // No-op — in-memory repositories commit immediately on Add.
        return Task.FromResult(0);
    }
}
