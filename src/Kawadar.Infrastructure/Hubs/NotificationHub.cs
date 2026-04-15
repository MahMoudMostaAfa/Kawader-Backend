using Microsoft.AspNetCore.SignalR;

namespace Kawadar.Infrastructure.Hubs;

public class NotificationHub : Hub
{


  public override async Task OnConnectedAsync()
  {
    var userId = Context.UserIdentifier;
    if (userId != null) await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId));
    else throw new HubException("Unauthorized");

    await base.OnConnectedAsync();
  }
  public override async Task OnDisconnectedAsync(Exception? exception)
  {
    var userId = Context.UserIdentifier;
    if (userId != null) await Groups.RemoveFromGroupAsync(Context.ConnectionId, UserGroup(userId));

    await base.OnDisconnectedAsync(exception);
  }



  public static string UserGroup(string userId) => $"notifications:user:{userId}";
}