
namespace Kawadar.Application.Common.Messaging.Messages;

using Microsoft.AspNetCore.Http;

public class UpdateProfileImageMessage
{

  public Guid UserProfileId { get; init; }
  public byte[] ProfilePicData { get; init; } = default!;
  public string FileName { get; init; } = default!;
  public string ContainerName { get; init; } = default!;

}