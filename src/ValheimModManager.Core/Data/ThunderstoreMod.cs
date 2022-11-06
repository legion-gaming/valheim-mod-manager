using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace ValheimModManager.Core.Data;

public class ThunderstoreMod
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("full_name")]
    public string FullName { get; set; }

    [JsonPropertyName("owner")]
    public string Owner { get; set; }

    [JsonPropertyName("package_url")]
    public string PackageUrl { get; set; }

    [JsonPropertyName("date_created")]
    public DateTime DateCreated { get; set; }

    [JsonPropertyName("date_updated")]
    public DateTime DateUpdated { get; set; }

    [JsonPropertyName("uuid4")]
    public string Uuid4 { get; set; }

    [JsonPropertyName("rating_score")]
    public int RatingScore { get; set; }

    [JsonPropertyName("is_pinned")]
    public bool IsPinned { get; set; }

    [JsonPropertyName("is_deprecated")]
    public bool IsDeprecated { get; set; }

    [JsonPropertyName("has_nsfw_content")]
    public bool HasNsfwContent { get; set; }

    [JsonPropertyName("categories")]
    public string[] Categories { get; set; }

    [JsonPropertyName("versions")]
    public ThunderstoreModVersion[] Versions { get; set; }

    [JsonPropertyName("donation_link")]
    public string DonationLink { get; set; }

    [JsonIgnore]
    public ThunderstoreModVersion Latest
    {
        get { return Versions.OrderByDescending(version => Version.Parse(version.VersionNumber)).First(); }
    }
}