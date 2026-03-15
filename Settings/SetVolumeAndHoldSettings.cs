using System.Text.Json.Serialization;

namespace SystemTools.Actions;

public class SetVolumeAndHoldSettings
{
    [JsonPropertyName("volumePercent")] public float VolumePercent { get; set; } = 50f;

    [JsonPropertyName("holdSeconds")] public float HoldSeconds { get; set; } = 5f;
}
