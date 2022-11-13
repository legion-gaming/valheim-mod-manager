using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ValheimModManager.Core.Data
{
    public class ThunderstoreDependency
    {
        public static implicit operator string(ThunderstoreDependency dependency)
        {
            return dependency.ToString();
        }

        public static implicit operator ThunderstoreDependency(string dependencyString)
        {
            return new ThunderstoreDependency(dependencyString);
        }

        private ThunderstoreDependency(string dependencyString)
        {
            if (!TryParse(dependencyString, out var mod))
            {
                throw new FormatException("Dependency string was not in a valid format.");
            }

            Author = mod.Author;
            Name = mod.Name;
            Version = mod.Version;
        }

        public string Author { get; }
        public string Name { get; }
        public Version Version { get; }

        public override string ToString()
        {
            return $"{Author}-{Name}-{Version}";
        }

        private static bool TryParse(string dependencyString, out (string Author, string Name, Version Version) mod)
        {
            mod = default;

            if (string.IsNullOrWhiteSpace(dependencyString))
            {
                return false;
            }

            if (dependencyString.Count(@char => @char.Equals('-')) > 2)
            {
                return TryFallbackParse(dependencyString, out mod);
            }

            var pattern = new Regex(@"(?<Author>[^-]+)-(?<Name>[^-]+)-(?<Version>\d+\.\d+\.\d+)");

            if (!pattern.IsMatch(dependencyString))
            {
                return false;
            }

            var match = pattern.Match(dependencyString);
            var author = match.Groups["Author"].Value;
            var name = match.Groups["Name"].Value;
            var version = Version.Parse(match.Groups["Version"].Value);

            mod = (author, name, version);

            return true;
        }

        private static bool TryFallbackParse(string dependencyString, out (string Author, string Name, Version Version) mod)
        {
            mod = default;

            var parts = dependencyString.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (parts.Count < 3)
            {
                return false;
            }

            if (!Version.TryParse(parts.Last(), out var version))
            {
                return false;
            }

            parts.Remove(parts.Last());

            var name = parts.Last();

            parts.Remove(parts.Last());

            var author = string.Join('-', parts);

            mod = (author, name, version);

            return true;
        }
    }
}
