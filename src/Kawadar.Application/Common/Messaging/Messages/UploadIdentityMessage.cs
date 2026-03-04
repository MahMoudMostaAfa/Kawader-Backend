namespace Kawadar.Application.Common.Messaging.Messages;


public class UploadIdentityMessage
{

  public Guid UserProfileId { get; init; }
  public byte[] IdentityFrontPicData { get; init; } = default!;
  public byte[] IdentityBackPicData { get; init; } = default!;
  public string FileName { get; init; } = default!;
  public string ContainerName { get; init; } = default!;

}