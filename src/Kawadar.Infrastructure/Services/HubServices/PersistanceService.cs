using System.Collections.Concurrent;
using Kawadar.Application.Common.Hubs;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kawadar.Infrastructure.Services.HubServices;


public class PersistanceService : IPersistanceService
{


  private readonly ILogger<PersistanceService> _logger;

  private readonly IServiceScopeFactory _scopeFactory;

  public PersistanceService(ILogger<PersistanceService> logger, IServiceScopeFactory scopeFactory)
  {
    _logger = logger;
    _scopeFactory = scopeFactory;
  }
  // In-memory: userId → set of 
  private static readonly ConcurrentDictionary<string, HashSet<string>> _connections = new();

  private static readonly object _lock = new();

  public async Task UserConnectedAsync(string userId, string connectionId, CancellationToken cancellationToken = default)
  {
    bool wasOffline;
    lock (_lock)
    {

      if (!_connections.TryGetValue(userId, out var conns))
      {

        conns = new HashSet<string>();
        _connections[userId] = conns;
      }
      wasOffline = conns.Count == 0;
      conns.Add(connectionId);
    }

    if (wasOffline)
    {
      _logger.LogInformation("User {UserId} connected with connection {ConnectionId}", userId, connectionId);

      // Update DB to set user Online
      using var scope = _scopeFactory.CreateScope();
      var usersRepository = scope.ServiceProvider.GetRequiredService<IUsersRepository>();
      var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

      var userProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
      if (userProfileResult.IsError)
      {
        _logger.LogError("Failed to retrieve user profile for user {UserId}: {Error}", userId, userProfileResult.Errors[0].Description);
        return;
      }
      var userProfile = userProfileResult.Value;
      var updateOnlineResult = userProfile.UpdateOnlineStatus(true, null);
      if (updateOnlineResult.IsError)
      {
        _logger.LogError("Failed to update online status for user {UserId}: {Error}", userId, updateOnlineResult.Errors[0].Description);
        return;
      }

      await unitOfWork.SaveChangesAsync(cancellationToken);
      _logger.LogInformation("User {UserId} is now online", userId);

    }


  }

  public async Task UserDisconnectedAsync(string userId, string connectionId, CancellationToken cancellationToken = default)
  {
    bool isNowOffline;

    lock (_lock)
    {
      if (_connections.TryGetValue(userId, out var conns))
      {
        conns.Remove(connectionId);
        isNowOffline = conns.Count == 0;
        if (isNowOffline) _connections.TryRemove(userId, out _);
      }
      else
      {
        isNowOffline = true;
      }
    }

    if (isNowOffline)
    {
      var lastSeen = DateTime.UtcNow;

      using var scope = _scopeFactory.CreateScope();
      var usersRepository = scope.ServiceProvider.GetRequiredService<IUsersRepository>();
      var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

      var userProfileResult = await usersRepository.GetUserProfileByUserIdAsync(userId);
      if (userProfileResult.IsError)
      {
        _logger.LogError("Failed to retrieve user profile for user {UserId}: {Error}", userId, userProfileResult.Errors[0].Description);
        return;
      }
      var userProfile = userProfileResult.Value;
      var updateOnlineResult = userProfile.UpdateOnlineStatus(false, lastSeen);
      if (updateOnlineResult.IsError)
      {
        _logger.LogError("Failed to update online status for user {UserId}: {Error}", userId, updateOnlineResult.Errors[0].Description);
        return;
      }

      await unitOfWork.SaveChangesAsync(cancellationToken);
      _logger.LogInformation("User {UserId} went offline at {LastSeen}", userId, lastSeen);


    }

  }

  public Task<bool> IsUserOnlineAsync(string userId, CancellationToken cancellationToken = default)
  {
    var isOnline = _connections.TryGetValue(userId, out var conns) && conns.Count > 0;

    return Task.FromResult(isOnline);
  }

  public Task<IEnumerable<string>> GetOnlineUsersAsync(IEnumerable<string> userIds, CancellationToken ct = default)
  {
    var online = userIds.Where(id =>
           _connections.TryGetValue(id, out var conns) && conns.Count > 0);
    return Task.FromResult(online);
  }


}