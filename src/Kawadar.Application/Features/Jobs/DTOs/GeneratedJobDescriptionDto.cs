using System.Text.Json.Serialization;

namespace Kawadar.Application.Features.Jobs.DTOs;

public class GeneratedJobDescriptionDto
{
  [JsonPropertyName("description")]
  public string Description { get; set; } = string.Empty;
}
