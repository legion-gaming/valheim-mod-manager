using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using ValheimModManager.Core.Comparers;
using ValheimModManager.Core.Data;
using ValheimModManager.Core.Helpers;

namespace ValheimModManager.Core.Services
{
    public class ThunderstoreService : IThunderstoreService
    {
        private readonly ThunderstoreClient _client;
        private readonly AsyncLazy<IReadOnlyList<ThunderstoreMod>> _cache;
        private readonly DependencyComparer _dependencyComparer;

        public ThunderstoreService()
        {
            var client = new ThunderstoreClient();

            _client = new ThunderstoreClient();
            _cache = new AsyncLazy<IReadOnlyList<ThunderstoreMod>>(() => client.GetModsAsync());
            _dependencyComparer = new DependencyComparer();
        }

        public async Task<IReadOnlyList<ThunderstoreMod>> GetModsAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var mods = await _cache.Value;

            return 
                mods.Where(mod => mod.Owner != "ebkr" && mod.Name != "r2modman")
                    .Where(mod => !mod.IsDeprecated)
                    .ToList();
        }

        public async Task<ThunderstoreModVersion> GetModAsync(ThunderstoreDependency dependency, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var mods = await GetModsAsync(cancellationToken);

            return mods.SelectMany(mod => mod.Versions).FirstOrDefault(mod => mod.FullName.Equals(dependency));
        }

        public Task<ZipArchive> DownloadModAsync(ThunderstoreDependency dependency, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var author = dependency.Author;
            var name = dependency.Name;
            var version = dependency.Version;
            var url = $"https://valheim.thunderstore.io/package/download/{author}/{name}/{version}/";

            return _client.DownloadModAsync(url, cancellationToken);
        }

        public async IAsyncEnumerable<string> ResolveDependenciesAsync(ThunderstoreDependency dependency, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var hashSet = new HashSet<string>(_dependencyComparer);

            await foreach (var resolvedDependency in ResolveDependenciesAsyncImpl(dependency, cancellationToken))
            {
                if (!hashSet.Add(resolvedDependency))
                {
                    hashSet.TryGetValue(resolvedDependency, out var existingDependency);

                    if (_dependencyComparer.Compare(resolvedDependency, existingDependency) > 0)
                    {
                        hashSet.Remove(existingDependency);
                        hashSet.Add(resolvedDependency);
                    }
                }
            }

            foreach (var resolvedDependency in hashSet)
            {
                yield return resolvedDependency;
            }
        }

        public async IAsyncEnumerable<string> ResolveBackwardDependenciesAsync(ThunderstoreDependency dependency, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string dependencyString = dependency;

            var mods = await GetModsAsync(cancellationToken);

            var dependencies =
                mods.SelectMany(mod => mod.Versions)
                    .Where(version => version.Dependencies.Contains(dependencyString, _dependencyComparer))
                    .Select(version => version.FullName);

            foreach (var mod in dependencies)
            {
                yield return mod;
            }
        }

        private async IAsyncEnumerable<string> ResolveDependenciesAsyncImpl(ThunderstoreDependency dependency, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var mods = await GetModsAsync(cancellationToken);

            var version =
                mods.SelectMany(mod => mod.Versions)
                    .First(mod => mod.FullName.Equals(dependency));

            foreach (var resolvedDependency in version.Dependencies)
            {
                yield return resolvedDependency;

                await foreach (var innerDependency in ResolveDependenciesAsyncImpl(resolvedDependency, cancellationToken))
                {
                    yield return innerDependency;
                }
            }
        }
    }
}
