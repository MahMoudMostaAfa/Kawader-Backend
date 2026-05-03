using Kawadar.Domain.WalletAndPayments;

namespace Kawadar.Application.Common.Interfaces.BackgroundJobs;

public interface IEscrowReleaseScheduler
{
  void ScheduleEscrowRelease(Guid escrowTransactionId, TimeSpan delay);
}
