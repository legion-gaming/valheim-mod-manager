using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using ValheimModManager.Core.Data;
using ValheimModManager.Core.Helpers;

namespace ValheimModManager.Core.Services
{
    public class ThunderstoreService : IThunderstoreService
    {
        private readonly ThunderstoreClient _client;
        private readonly AsyncLazy<IReadOnlyList<ThunderstoreMod>> _modCache;

        public ThunderstoreService(ThunderstoreClient client)
        {
            _client = client;
            _modCache = new AsyncLazy<IReadOnlyList<ThunderstoreMod>>(() => client.GetModsAsync());
        }

        public async Task<IReadOnlyList<ThunderstoreMod>> GetModsAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var mods = await _modCache.Value;

            return
                mods.Where(mod => !mod.Name.Equals("r2modman", StringComparison.OrdinalIgnoreCase))
                    .ToList();
        }

        public async Task<ThunderstoreModVersion> GetModAsync(string dependencyString, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var mods = await _modCache.Value;

            return
                mods.SelectMany(mod => mod.Versions)
                    .FirstOrDefault(version => version.FullName.Equals(dependencyString));
        }

        public async Task<IReadOnlyList<ThunderstoreModVersion>> GetInstalledModsAsync(string profileName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!File.Exists(PathHelper.GetProfilePath(profileName)))
            {
                return new List<ThunderstoreModVersion>();
            }

            using (var profileStream = File.Open(PathHelper.GetProfilePath(profileName), FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (profileStream.Length == 0)
                {
                    return new List<ThunderstoreModVersion>();
                }

                profileStream.Seek(0, SeekOrigin.Begin);

                var profile = await JsonSerializer.DeserializeAsync<List<string>>(profileStream, cancellationToken: cancellationToken);
                var mods = await _modCache.Value;

                return
                    mods.SelectMany(mod => mod.Versions)
                        .Join
                        (
                            profile!,
                            version => version.FullName,
                            dependencyString => dependencyString,
                            (version, _) => version
                        )
                        .ToList();
            }
        }

        public Task<ZipArchive> DownloadModAsync(string url, CancellationToken cancellationToken = default)
        {
            return _client.DownloadModAsync(url, cancellationToken);
        }
    }
}
