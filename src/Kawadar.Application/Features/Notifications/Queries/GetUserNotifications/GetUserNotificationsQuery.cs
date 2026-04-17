using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Notifications.Queries.GetUserNotifications;

public record GetUserNotificationsQuery(int Page, int PageSize) : IRequest<Result<PaginatedList<NotificationDto>>>;