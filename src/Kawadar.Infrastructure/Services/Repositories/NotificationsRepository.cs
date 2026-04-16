using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.ConversastionsAndMessages.EventHandlers;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Notifications;
using Kawadar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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

  public async Task<Result<Notification>> GetByIdAsync(Guid notificationId, CancellationToken cancellationToken = default)
  {
    var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId, cancellationToken);
    if (notification is null) return Error.NotFound("Notifications.NotFound", "Notification not found.");
    return notification;
  }

  public async Task<Result<IEnumerable<Notification>>> GetUserNotificationsAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    var notifications = await _context.Notifications.Where(n => n.UserId == userId).ToListAsync(cancellationToken);
    return notifications;
  }

  public async Task<Result<PaginatedList<Notification>>> GetUserNotificationsAsync(Guid userId, int page, int pageSize)
  {
    var count = _context.Notifications.Count(n => n.UserId == userId);

    var notifications = await _context.Notifications.Where(n => n.UserId == userId)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .OrderByDescending(n => n.CreatedAt)
        .ToListAsync();


    var paginatedList = new PaginatedList<Notification>(notifications, count, page, pageSize);
    return paginatedList;
  }
}