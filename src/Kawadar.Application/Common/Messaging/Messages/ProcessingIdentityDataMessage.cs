namespace Kawadar.Application.Common.Messaging.Messages;

public class ProcessingIdentityDataMessage
{

  public Guid UserProfileId { get; init; }
  public byte[] IdentityFrontPicData { get; init; } = default!;



}