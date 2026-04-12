using Kawadar.Application.Common.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Kawadar.Infrastructure.Hubs;

[Authorize]
public class PersistanceHub : Hub
{
  private readonly ILogger<PersistanceHub> _logger;

  private readonly IPersistanceService _persistanceService;

  public PersistanceHub(ILogger<PersistanceHub> logger, IPersistanceService persistanceService)
  {
    _logger = logger;
    _persistanceService = persistanceService;
  }

  public override async Task OnConnectedAsync()
  {
    var userId = Context.UserIdentifier ?? throw new HubException("Unauthorized");

    _logger.LogInformation("User {UserId} is connecting to PersistanceHub with connection {ConnectionId}", userId, Context.ConnectionId);


    await _persistanceService.UserConnectedAsync(userId, Context.ConnectionId);
    await base.OnConnectedAsync();
  }


  public override async Task OnDisconnectedAsync(Exception? exception)
  {
    var userId = Context.UserIdentifier ?? throw new HubException("Unauthorized");

    _logger.LogInformation("User {UserId} disconnected from PersistanceHub", userId);
    await _persistanceService.UserDisconnectedAsync(userId, Context.ConnectionId);
    await base.OnDisconnectedAsync(exception);
  }



}