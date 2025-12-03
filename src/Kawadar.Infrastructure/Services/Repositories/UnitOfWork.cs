using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Infrastructure.Data;

namespace Kawadar.Infrastructure.Services.Repositories;

public class UnitOfWork(AppDbContext appDbContext) : IUnitOfWork
{
  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    return await appDbContext.SaveChangesAsync(cancellationToken);
  }
}