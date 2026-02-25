namespace Kawadar.Application.Common.Messaging.Messages;

public record SendWelcomeEmailMessage(string Email, string FullName)
{
  // Parameterless constructor required by MassTransit for message deserialization
  public SendWelcomeEmailMessage() : this(string.Empty, string.Empty) { }
};