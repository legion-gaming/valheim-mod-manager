using System;
using System.Collections.Generic;
using System.Linq;

using ValheimModManager.Core.Data;

namespace ValheimModManager.Pages.Models
{
    public class Mod
    {
        public static implicit operator Mod(ThunderstoreMod mod)
        {
            return new Mod(mod);
        }

        public static implicit operator Mod(ThunderstoreModVersion version)
        {
            return new Mod(version);
        }

        private Mod(ThunderstoreMod mod)
        {
            Author = mod.Owner;
            Name = mod.Name;
            Description = mod.Latest.Description;
            Icon = mod.Latest.Icon;
            LastUpdated = mod.DateUpdated;
            Dependency = mod.Latest.FullName;
            Version = mod.Latest;
            Versions = mod.Versions;
        }

        private Mod(ThunderstoreModVersion version)
        {
            Author = version.Author;
            Name = version.Name;
            Description = version.Description;
            Icon = version.Icon;
            LastUpdated = version.DateCreated;
            Dependency = version.FullName;
            Version = version;
            Versions = Enumerable.Empty<ThunderstoreModVersion>();
        }

        public string Author { get; }
        public string Name { get; }
        public string Description { get; }
        public string Icon { get; }
        public DateTime LastUpdated { get; }
        public ThunderstoreDependency Dependency { get; }
        public ThunderstoreModVersion Version { get; }
        public IEnumerable<ThunderstoreModVersion> Versions { get; internal set; }
    }
}
