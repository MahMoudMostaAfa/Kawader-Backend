using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Commands.DeleteMessage;


public record DeleteMessageCommand(string? userId, string? connectionId, Guid messageId) : IRequest<Result<MessageDto>>;