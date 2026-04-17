using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Commands.DeleteConversation;

public record DeleteConversationCommand(Guid ConversationId) : IRequest<Result<Deleted>>;