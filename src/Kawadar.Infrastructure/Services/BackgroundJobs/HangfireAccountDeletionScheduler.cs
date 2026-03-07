using Hangfire;
using Kawadar.Application.Common.Interfaces.BackgroundJobs;

namespace Kawadar.Infrastructure.Services.BackgroundJobs;

public class HangfireAccountDeletionScheduler : IAccountDeletionScheduler
{
  private readonly IBackgroundJobClient _backgroundJobClient;

  public HangfireAccountDeletionScheduler(IBackgroundJobClient backgroundJobClient)
  {
    _backgroundJobClient = backgroundJobClient;
  }

  public void SchedulePermanentDeletion(string userId, TimeSpan delay)
  {
    _backgroundJobClient.Schedule<PermanentAccountDeletionJob>(
      job => job.ExecuteAsync(userId, CancellationToken.None),
      delay);
  }
}
