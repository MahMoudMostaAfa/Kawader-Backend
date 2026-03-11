using System.Text.Json.Serialization;

namespace Kawadar.Domain.Proposals.Enums;

public enum JobProposalType
{
  [JsonPropertyName("one_time")]
  OneTime = 1,
  [JsonPropertyName("milestone_based")]
  MilestoneBased = 2,
  [JsonPropertyName("hourly")]
  Hourly = 3
}