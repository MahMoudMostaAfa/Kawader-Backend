using Kawadar.Domain.Common.Results;

namespace Kawadar.Application.Common.Hubs;


public interface IPersistanceService
{

  Task UserConnectedAsync(string userId, string connectionId, CancellationToken cancellationToken = default);
  Task UserDisconnectedAsync(string userId, string connectionId, CancellationToken cancellationToken = default);

  Task<bool> IsUserOnlineAsync(string userId, CancellationToken cancellationToken = default);


  Task<IEnumerable<string>> GetOnlineUsersAsync(IEnumerable<string> userIds, CancellationToken ct = default);
}