using Kawadar.Domain.Badges;
using Kawadar.Domain.Badges.FreelancerBadges;
using Kawadar.Domain.Common;
using Kawadar.Domain.Jobs;
using Kawadar.Domain.Jobs.JobFiles;
using Kawadar.Domain.Jobs.JobQuestions;
using Kawadar.Domain.Jobs.JobReports;
using Kawadar.Domain.Portfolios.Items;
using Kawadar.Domain.Portfolios.Project;
using Kawadar.Domain.Portfolios.ProjectSkill;
using Kawadar.Domain.Portfolios.ProjectView;
using Kawadar.Domain.Reviews;
using Kawadar.Domain.Skills;
using Kawadar.Domain.Skills.FreelancerSkill;
using Kawadar.Domain.Specilizations;
using Kawadar.Domain.UserProfiles;
using Kawadar.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kawadar.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator) : IdentityDbContext<AppUser>(options)
{

  public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
  public DbSet<PortfolioItem> PortfolioItems => Set<PortfolioItem>();
  public DbSet<PortfolioProject> PortfolioProjects => Set<PortfolioProject>();
  public DbSet<PortfolioProjectSkill> ProjectSkills => Set<PortfolioProjectSkill>();
  public DbSet<PortfolioProjectView> ProjectViews => Set<PortfolioProjectView>();
  public DbSet<Specilization> Specilizations => Set<Specilization>();
  public DbSet<Badge> Badges => Set<Badge>();
  public DbSet<FreelancerBadge> FreelancerBadges => Set<FreelancerBadge>();
  public DbSet<Skill> Skills => Set<Skill>();
  public DbSet<FreelancerSkill> FreelacnerSkills => Set<FreelancerSkill>();
  public DbSet<Job> Jobs => Set<Job>();
  public DbSet<JobQuestion> JobQuestions => Set<JobQuestion>();
  public DbSet<JobFile> JobFiles => Set<JobFile>();
  public DbSet<JobReport> JobReports => Set<JobReport>();
  public DbSet<Review> Reviews => Set<Review>();




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