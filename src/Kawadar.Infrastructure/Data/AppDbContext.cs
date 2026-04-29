using Kawadar.Domain.Badges;
using Kawadar.Domain.Badges.FreelancerBadges;
using Kawadar.Domain.Common;
using Kawadar.Domain.Jobs;
using Kawadar.Domain.Jobs.JobFiles;
using Kawadar.Domain.Jobs.JobQuestions;
using Kawadar.Domain.Jobs.JobReports;
using Kawadar.Domain.Jobs.JobViews;
using Kawadar.Domain.Portfolios.Items;
using Kawadar.Domain.Portfolios.Project;
using Kawadar.Domain.Portfolios.ProjectSkill;
using Kawadar.Domain.Portfolios.ProjectView;
using Kawadar.Domain.Reviews;
using Kawadar.Domain.Proposals;
using Kawadar.Domain.Proposals.ProposalMilestones;
using Kawadar.Domain.Proposals.QuestionAnswers;
using Kawadar.Domain.Skills;
using Kawadar.Domain.Skills.FreelancerSkill;
using Kawadar.Domain.Specilizations;
using Kawadar.Domain.UserProfiles;
using Kawadar.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Kawadar.Domain.Jobs.SavedJobs;
using Kawadar.Domain.Conversations.Messages;
using Kawadar.Domain.Notifications;
using Kawadar.Domain.Conversations;
using Kawadar.Domain.UserProfiles.UserReports;
using Kawadar.Domain.Contracts;
using Kawadar.Domain.Subscriptions;
using Kawadar.Domain.Contracts.Disbutes;
using Kawadar.Domain.Violations;
using Kawadar.Domain.WalletAndPayments;
using Kawadar.Domain.WalletAndPayments.Payouts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

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
  public DbSet<UserReport> UserReports => Set<UserReport>();
  public DbSet<Review> Reviews => Set<Review>();
  public DbSet<JobView> JobViews => Set<JobView>();

  public DbSet<JobProposal> JobProposals => Set<JobProposal>();

  public DbSet<SavedJob> SavedJobs => Set<SavedJob>();

  public DbSet<ProposalMilestone> ProposalMilestones => Set<ProposalMilestone>();

  public DbSet<ProposalQuestionAnswer> ProposalQuestionAnswers => Set<ProposalQuestionAnswer>();


  public DbSet<Message> Messages => Set<Message>();
  public DbSet<MessageFile> MessageFiles => Set<MessageFile>();

  public DbSet<Notification> Notifications => Set<Notification>();

  public DbSet<Conversation> Conversations => Set<Conversation>();
  public DbSet<Contract> Contracts => Set<Contract>();
  public DbSet<Disbute> Disbutes => Set<Disbute>();
  public DbSet<Violation> Violations => Set<Violation>();
  public DbSet<ContractMilestone> ContractMilestones => Set<ContractMilestone>();

  public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
  public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();

  public DbSet<Wallet> Wallets => Set<Wallet>();
  public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();

  public DbSet<EscrowTransaction> EscrowTransactions => Set<EscrowTransaction>();

  public DbSet<WithdrawalRequest> WithdrawalRequests => Set<WithdrawalRequest>();

  public DbSet<UserPayoutAccount> UserPayoutAccounts => Set<UserPayoutAccount>();



  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    await DispatchDomainEventsAsync(cancellationToken);

    // for testing purpose

    // TEMPORARY: Log all tracked entity states before saving

    // foreach (var entry in ChangeTracker.Entries())
    // {
    //   if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
    //   {
    //     var concurrencyProps = entry.Properties
    //       .Where(p => p.Metadata.IsConcurrencyToken)
    //       .Select(p => $"{p.Metadata.Name}={p.CurrentValue ?? "null"}(orig={p.OriginalValue ?? "null"})")
    //       .ToList();

    //     logger.LogWarning("[EF SAVE] {Entity} | State={State} | ConcurrencyTokens=[{Tokens}]",
    //       entry.Entity.GetType().Name, entry.State, string.Join(", ", concurrencyProps));
    //   }
    // }

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


    // Fix: Clear the events FIRST before publishing them.
    // This prevents the infinite loop if a handler calls SaveChangesAsync().
    foreach (var entity in domainEntities)
    {
      entity.ClearDomainEvents();
    }

    foreach (var domainEvent in domainEvents)
    {
      await mediator.Publish(domainEvent, cancellationToken);
    }


  }

}