namespace Kawadar.Application.Common.Interfaces.BackgroundJobs;

public interface IAccountDeletionScheduler
{
  void SchedulePermanentDeletion(string userId, TimeSpan delay);
}
