using Kawadar.Application.Features.ConversastionsAndMessages.EventHandlers;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Notifications;
using Kawadar.Infrastructure.Data;

namespace Kawadar.Infrastructure.Services.Repositories;


public class NotificationsRepository : INotificationsRepository
{

  private readonly AppDbContext _context;

  public NotificationsRepository(AppDbContext context)
  {
    _context = context;


  }

  public async Task<Result<Created>> AddNotificationAsync(Notification notification, CancellationToken cancellationToken)
  {
    await _context.Notifications.AddAsync(notification, cancellationToken);
    return Result.Created;
  }
}