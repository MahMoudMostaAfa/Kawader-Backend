using Microsoft.AspNetCore.Http;

namespace Kawadar.Application.Features.Jobs.DTOs;

/// <summary>
/// Represents a single job attachment which is either an uploaded file (zip, image, pdf)
/// or an external link. Exactly one of <see cref="File"/> or <see cref="ExternalUrl"/> must be set.
/// </summary>
public record CreateJobAttachmentDto
{
  /// <summary>File upload – zip / image / pdf.</summary>
  public IFormFile? File { get; init; }

  /// <summary>External URL when the attachment is a link.</summary>
  public string? ExternalUrl { get; init; }

  public bool IsFile => File is not null;
  public bool IsLink => ExternalUrl is not null && File is null;
}
