using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Commands.EditMessage;

public record EditMessageCommand(string? userId, string? connectionId, Guid messageId, string newContent) : IRequest<Result<MessageDto>>;