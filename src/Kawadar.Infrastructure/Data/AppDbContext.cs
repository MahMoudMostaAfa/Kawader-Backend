using Kawadar.Domain.Common;
using Kawadar.Domain.UserProfiles;
using Kawadar.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kawadar.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator) : IdentityDbContext<AppUser>(options)
{

  public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    await DispatchDomainEventsAsync(cancellationToken);
    return await base.SaveChangesAsync(cancellationToken);
  }
  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
  }

  private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
  {
    var domainEntities = ChangeTracker.Entries()
        .Where(e => e.Entity is Entity baseEntity && baseEntity.DomainEvents.Count != 0)
        .Select(e => (Entity)e.Entity)
        .ToList();

    var domainEvents = domainEntities
        .SelectMany(e => e.DomainEvents)
        .ToList();

    foreach (var domainEvent in domainEvents)
    {
      await mediator.Publish(domainEvent, cancellationToken);
    }

    foreach (var entity in domainEntities)
    {
      entity.ClearDomainEvents();
    }
  }

}