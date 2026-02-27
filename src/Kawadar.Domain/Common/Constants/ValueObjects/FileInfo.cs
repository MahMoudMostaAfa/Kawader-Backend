namespace Kawadar.Domain.Common.ValueObjects;


public class FileInfo
{
  public string FileName { get; set; } = "";
  public string FileUrl { get; set; } = "";
  public long FileSizeInBytes { get; set; }
  public string MimeType { get; set; } = "";
}