namespace Kawadar.Api.Requests.Job;

public class AddJobAttachmentRequest
{
  public IFormFile? File { get; set; }
  public string? ExternalUrl { get; set; }
}
