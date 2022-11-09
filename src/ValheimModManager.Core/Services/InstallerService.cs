using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using ValheimModManager.Core.Helpers;
using ValheimModManager.Core.Utilities;

namespace ValheimModManager.Core.Services
{
    public class InstallerService : IInstallerService
    {
        private readonly IThunderstoreService _thunderstoreService;

        public InstallerService(IThunderstoreService thunderstoreService)
        {
            _thunderstoreService = thunderstoreService;
        }

        public async Task InstallAsync(string profileName, string dependencyString, bool skipDependencies, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var installManifest = BuildInstallManifestAsync(dependencyString, skipDependencies, cancellationToken);
            var installedMods = await _thunderstoreService.GetInstalledModsAsync(profileName, cancellationToken);

            var profile =
                new List<ModInstallInfo>
                (
                    installedMods.Select(mod => new ModInstallInfo(mod.FullName, mod.DownloadUrl))
                );

            await foreach (var install in installManifest.WithCancellation(cancellationToken))
            {
                var zipArchive = await _thunderstoreService.DownloadModAsync(install.Url, cancellationToken);
                var zipExtractorFactory = new ZipExtractorFactory(zipArchive, PathHelper.GetProfileBasePath(profileName));
                var zipExtractor = zipExtractorFactory.Create(install.DependencyString);

                await zipExtractor.ExtractAsync(cancellationToken);

                profile.Add(install);
            }

            var hashSet = new HashSet<string>();

            profile =
                profile.OrderBy(mod => mod.Name)
                    .ThenByDescending(mod => mod.Version)
                    .Where
                    (
                        mod =>
                        {
                            return hashSet.Add($"{mod.Author}-{mod.Name}");
                        }
                    )
                    .ToList();

            using (var profileStream = File.Open(PathHelper.GetProfilePath(profileName), FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (profileStream.Length == 0)
                {
                    await JsonSerializer.SerializeAsync
                    (
                        profileStream,
                        profile.Select(mod => mod.DependencyString),
                        cancellationToken: cancellationToken
                    );

                    return;
                }

                await JsonSerializer.SerializeAsync
                (
                    profileStream,
                    profile.Select(mod => mod.DependencyString),
                    cancellationToken: cancellationToken
                );
            }
        }

        public async Task UninstallAsync(string profileName, string dependencyString, bool skipDependencies, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var installManifest = BuildInstallManifestAsync(dependencyString, skipDependencies, cancellationToken);
            var installedMods = await _thunderstoreService.GetInstalledModsAsync(profileName, cancellationToken);

            var profile =
                new List<ModInstallInfo>
                (
                    installedMods.Select(mod => new ModInstallInfo(mod.FullName, mod.DownloadUrl))
                );

            await foreach (var install in installManifest.WithCancellation(cancellationToken))
            {
                try
                {
                    var folder = PathHelper.GetBepInExPluginBasePath(profileName);
                    folder = Path.Combine(folder, $"{install.Name}-{install.Author}");

                    Directory.Delete(folder, true);
                }
                catch
                {
                    // Suppress error
                }

                profile.Remove(profile.First(x => x.DependencyString == install.DependencyString));
            }

            profile =
                profile.OrderBy(mod => mod.Name)
                    .ThenByDescending(mod => mod.Version)
                    .Where
                    (
                        mod =>
                        {
                            var hashSet = new HashSet<string>();
                            return hashSet.Add($"{mod.Author}-{mod.Name}");
                        }
                    )
                    .ToList();

            using (var profileStream = File.Open(PathHelper.GetProfilePath(profileName), FileMode.Truncate, FileAccess.Write))
            {
                profileStream.Seek(0, SeekOrigin.Begin);

                if (profileStream.Length == 0)
                {
                    await JsonSerializer.SerializeAsync
                    (
                        profileStream,
                        profile.Select(mod => mod.DependencyString).ToList(),
                        cancellationToken: cancellationToken
                    );

                    return;
                }

                await JsonSerializer.SerializeAsync
                (
                    profileStream,
                    profile.Select(mod => mod.DependencyString).ToList(),
                    cancellationToken: cancellationToken
                );
            }
        }

        private async IAsyncEnumerable<ModInstallInfo> BuildInstallManifestAsync(string dependencyString, bool skipDependencies, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var mod = await _thunderstoreService.GetModAsync(dependencyString, cancellationToken);
            var modInstallManifest = new ModInstallInfo(dependencyString, mod.DownloadUrl);

            if (skipDependencies)
            {
                yield return modInstallManifest;
                yield break;
            }

            var hashSet =
                new HashSet<ModInstallInfo>(new ModInstallInfoComparer())
                {
                    modInstallManifest
                };

            var dependencies =
                BuildInstallManifestAsyncImpl(dependencyString, cancellationToken)
                    .OrderByDescending(manifest => manifest.Version);

            await foreach (var dependency in dependencies)
            {
                hashSet.Add(dependency);
            }

            foreach (var dependency in hashSet)
            {
                yield return dependency;
            }
        }

        private async IAsyncEnumerable<ModInstallInfo> BuildInstallManifestAsyncImpl(string dependencyString, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var mod = await _thunderstoreService.GetModAsync(dependencyString, cancellationToken);

            if (mod == null)
            {
                yield break;
            }

            foreach (var dependency in mod.Dependencies)
            {
                var dependencyMod = await _thunderstoreService.GetModAsync(dependency, cancellationToken);

                yield return new ModInstallInfo(dependency, dependencyMod.DownloadUrl);

                await foreach (var innerDependency in BuildInstallManifestAsyncImpl(dependency, cancellationToken))
                {
                    yield return innerDependency;
                }
            }
        }

        private class ModInstallInfo
        {
            public ModInstallInfo(string dependencyString, string url)
            {
                if (DependencyStringHelper.TryParse(dependencyString, out var mod))
                {
                    Author = mod.Author;
                    Name = mod.Name;
                    Version = mod.Version;
                }

                Url = url;
                DependencyString = dependencyString;
            }

            public string Author { get; }
            public string Name { get; }
            public Version Version { get; }
            public string Url { get; }
            public string DependencyString { get; }
        }

        private class ModInstallInfoComparer : IEqualityComparer<ModInstallInfo>
        {
            public bool Equals(ModInstallInfo x, ModInstallInfo y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(x, null))
                {
                    return false;
                }

                if (ReferenceEquals(y, null))
                {
                    return false;
                }

                if (x.GetType() != y.GetType())
                {
                    return false;
                }

                return x.Author == y.Author && x.Name == y.Name;
            }

            public int GetHashCode(ModInstallInfo modInstallManifest)
            {
                return HashCode.Combine(modInstallManifest.Author, modInstallManifest.Name);
            }
        }
    }
}
