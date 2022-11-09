using System;
using System.Text.Json.Serialization;

using ValheimModManager.Core.Helpers;

namespace ValheimModManager.Core.Data
{
    public class ThunderstoreModVersion
    {
        [JsonIgnore]
        public string Author
        {
            get
            {
                DependencyStringHelper.TryParse(FullName, out var mod);

                return mod.Author;
            }
        }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("full_name")]
        public string FullName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("version_number")]
        public string VersionNumber { get; set; }

        [JsonPropertyName("dependencies")]
        public string[] Dependencies { get; set; }

        [JsonPropertyName("download_url")]
        public string DownloadUrl { get; set; }

        [JsonPropertyName("downloads")]
        public int Downloads { get; set; }

        [JsonPropertyName("date_created")]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("website_url")]
        public string WebsiteUrl { get; set; }

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("uuid4")]
        public string Uuid4 { get; set; }

        [JsonPropertyName("file_size")]
        public int FileSize { get; set; }
    }
}
