using Hangfire;
using Kawadar.Application.Common.Interfaces.BackgroundJobs;
using Kawadar.Domain.WalletAndPayments;

namespace Kawadar.Infrastructure.Services.BackgroundJobs;

public class HangfireEscrowReleaseScheduler : IEscrowReleaseScheduler
{
  private readonly IBackgroundJobClient _backgroundJobClient;
  public HangfireEscrowReleaseScheduler(IBackgroundJobClient backgroundJobClient)
  {
    _backgroundJobClient = backgroundJobClient;
  }
  public void ScheduleEscrowRelease(Guid escrowTransactionId, TimeSpan delay)
  {

    _backgroundJobClient.Schedule<EscrowReleaseJob>(job => job.ExecuteAsync(escrowTransactionId, CancellationToken.None), delay);
  }
}