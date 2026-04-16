using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Commands.SendMessage;


public record SendMessageCommand(
 string? SenderId,
 string? connectionId,
 Guid conversationId,
 string content,
 Guid? replyToMessageId,
 List<IFormFile>? AttachmentFiles,
 List<string>? AttachmentLinks

) : IRequest<Result<MessageDto>>;