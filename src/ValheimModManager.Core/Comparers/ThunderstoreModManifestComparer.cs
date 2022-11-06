using System;
using System.Collections.Generic;

namespace ValheimModManager.Core.Comparers
{
    public class ThunderstoreModManifestComparer : IEqualityComparer<(string Author, string Name, Version Version)>
    {
        public bool Equals((string Author, string Name, Version Version) x, (string Author, string Name, Version Version) y)
        {
            return x.Item1 == y.Item1 && x.Item2 == y.Item2 && Equals(x.Item3, y.Item3);
        }

        public int GetHashCode((string Author, string Name, Version Version) obj)
        {
            return HashCode.Combine(obj.Item1, obj.Item2, obj.Item3);
        }
    }
}