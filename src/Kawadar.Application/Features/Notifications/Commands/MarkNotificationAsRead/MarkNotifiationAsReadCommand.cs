using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Notifications.Commands.MarkNotifiactionAsRead;

public record MarkNotifiationAsReadCommand(Guid NotificationId) : IRequest<Result<Updated>>;