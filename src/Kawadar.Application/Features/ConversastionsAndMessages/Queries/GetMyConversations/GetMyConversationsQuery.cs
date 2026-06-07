using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Queries.GetMyConversations;


public record GetMyConversationsQuery(int pageNumber, int pageSize) : IRequest<Result<PaginatedList<ConversationDto>>>;