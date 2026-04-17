
using Kawadar.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead;
using Kawadar.Application.Features.Notifications.Commands.MarkNotifiactionAsRead;
using Kawadar.Application.Features.Notifications.Queries.GetUserNotifications;
using MassTransit.RetryPolicies;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawadar.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/notifications")]
[Authorize]
public class NotificationsController : ApiController
{

  private readonly ISender _sender;

  public NotificationsController(ISender sender)
  {

    _sender = sender;


  }
  [HttpPut("{notificationId:guid}/mark-as-read")]
  public async Task<IActionResult> MarkAsRead(Guid notificationId)
  {
    var command = new MarkNotifiationAsReadCommand(notificationId);
    var result = await _sender.Send(command);


    return result.Match(
        _ => NoContent(),
        failure => Problem(failure)
    );
  }


  [HttpPut("mark-all-as-read")]
  public async Task<IActionResult> MarkAllAsRead()
  {
    var command = new MarkAllNotificationsAsReadCommand();
    var result = await _sender.Send(command);

    return result.Match(
        _ => NoContent(),
        failure => Problem(failure)
    );



  }

  [HttpGet]
  public async Task<IActionResult> GetUserNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
  {
    var query = new GetUserNotificationsQuery(page, pageSize);

    var result = await _sender.Send(query);

    return result.Match(
        notifications => Ok(notifications),
        failure => Problem(failure)
    );
  }
}