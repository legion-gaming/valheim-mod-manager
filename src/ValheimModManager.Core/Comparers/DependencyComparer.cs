using System.Collections.Generic;

using ValheimModManager.Core.Data;

namespace ValheimModManager.Core.Comparers
{
    public class DependencyComparer : IEqualityComparer<string>, IComparer<string>
    {
        public bool Equals(string x, string y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            ThunderstoreDependency left = x;
            ThunderstoreDependency right = y;

            return left.Author == right.Author && left.Name == right.Name;
        }

        public int GetHashCode(string obj)
        {
            return -1;
        }

        public int Compare(string x, string y)
        {
            ThunderstoreDependency left = x;
            ThunderstoreDependency right = y;

            if (left.Version > right.Version)
            {
                return 1;
            }

            if (left.Version < right.Version)
            {
                return -1;
            }

            return 0;
        }
    }
}
