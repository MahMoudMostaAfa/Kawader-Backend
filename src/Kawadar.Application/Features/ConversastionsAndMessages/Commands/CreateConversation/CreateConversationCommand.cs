using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Commands.CreateConversation;


public record CreateConversationCommand(string ReceiverUserName, Guid ProposalId, string Title, string InitialMessageContent) : IRequest<Result<Guid>>;