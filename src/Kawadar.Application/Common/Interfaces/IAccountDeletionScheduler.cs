namespace Kawadar.Application.Common.Interfaces;

public interface IAccountDeletionScheduler
{
  void SchedulePermanentDeletion(string userId, TimeSpan delay);
}
