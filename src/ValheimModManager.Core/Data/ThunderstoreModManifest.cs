using System.Text.Json.Serialization;

namespace ValheimModManager.Core.Data;

public class ThunderstoreModManifest
{
    [JsonIgnore]
    public string Author { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("version_number")]
    public string VersionNumber { get; set; }

    [JsonPropertyName("website_url")]
    public string WebsiteUrl { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("dependencies")]
    public string[] Dependencies { get; set; }
}