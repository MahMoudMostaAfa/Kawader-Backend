namespace Kawadar.Infrastructure.Services.HubServices.SignalRDTOs;

public record TypingIndicator(Guid ConversationId, string UserId, bool IsTyping);