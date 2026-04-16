using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead;

public record MarkAllNotificationsAsReadCommand : IRequest<Result<Updated>>;