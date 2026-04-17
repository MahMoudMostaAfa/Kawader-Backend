using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Conversations;


public static class ConversationErrors
{

  public static Error SenderAndReceiverCannotBeTheSame => Error.Conflict("SenderAndReceiverCannotBeTheSame", "Sender and receiver cannot be the same user.");


  public static Error MessageDoesNotBelongToConversation => Error.Conflict("MessageDoesNotBelongToConversation", "The message does not belong to this conversation.");
}