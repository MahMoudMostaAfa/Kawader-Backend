namespace Kawadar.Application.Common.Helpers;

using Kawadar.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

public static class MapIFormToFileData
{
  public static async Task<FileData> MapToFileData(IFormFile file)
  {
    using var memoryStream = new MemoryStream();

    // Copy the uploaded file stream into memory
    await file.CopyToAsync(memoryStream);

    return new FileData(
        Data: memoryStream.ToArray(),
        MimeType: file.ContentType // e.g., "image/jpeg" or "image/png"
    );
  }
}