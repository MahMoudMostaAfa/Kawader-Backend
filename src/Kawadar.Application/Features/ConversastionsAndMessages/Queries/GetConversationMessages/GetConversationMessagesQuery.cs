using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Queries.GetConversationMessages;

public record GetConversationMessagesQuery(Guid conversationId, int PageNumber, int PageSize) : IRequest<Result<ConversationMessagesDto>>;