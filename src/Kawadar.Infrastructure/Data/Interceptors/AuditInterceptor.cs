using Kawadar.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Kawadar.Infrastructure.Data.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
  private readonly TimeProvider _timeProvider;
  public AuditInterceptor(TimeProvider timeProvider)
  {
    _timeProvider = timeProvider;
  }
  public override InterceptionResult<int> SavingChanges(DbContextEventData eventData,
        InterceptionResult<int> result)
  {

    UpdateEntities(eventData.Context);

    return base.SavingChanges(eventData, result);
  }
  public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
         DbContextEventData eventData,
         InterceptionResult<int> result,
         CancellationToken cancellationToken = default)
  {
    UpdateEntities(eventData.Context);
    return base.SavingChangesAsync(eventData, result, cancellationToken);
  }


  private void UpdateEntities(DbContext? context)
  {

    if (context == null) return;
    var DateInUtc = _timeProvider.GetUtcNow().UtcDateTime;

    var entries = context.ChangeTracker.Entries<AuditableEntity>();

    foreach (var entry in entries)
    {
      if (entry.State == EntityState.Added)
      {
        entry.Entity.CreatedAt = DateInUtc;
        entry.Entity.UpdatedAt = DateInUtc;
      }
      else if (entry.State == EntityState.Modified)
      {
        entry.Entity.UpdatedAt = DateInUtc;
      }

    }
  }
}