using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ValheimModManager.Core.Comparers;
using ValheimModManager.Core.Data;
using ValheimModManager.Core.Helpers;
using ValheimModManager.Core.Utilities;

namespace ValheimModManager.Core.Services
{
    public class InstallerService : IInstallerService
    {
        private readonly IThunderstoreService _thunderstoreService;
        private readonly IProfileService _profileService;
        private readonly ZipExtractorFactory _zipExtractorFactory;

        public InstallerService(IThunderstoreService thunderstoreService, IProfileService profileService)
        {
            _thunderstoreService = thunderstoreService;
            _zipExtractorFactory = new ZipExtractorFactory();
            _profileService = profileService;
        }

        public async Task InstallAsync(string profileName, ThunderstoreDependency dependency, bool skipDependencies, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var zipArchive = await _thunderstoreService.DownloadModAsync(dependency, cancellationToken);
            var zipExtractor = _zipExtractorFactory.Create(dependency, zipArchive, PathHelper.GetProfileBasePath(profileName));

            await zipExtractor.ExtractAsync(cancellationToken);
            await _profileService.AddInstalledModAsync(profileName, dependency, cancellationToken);

            if (skipDependencies)
            {
                return;
            }

            var installedMods = await _profileService.GetInstalledModsAsync(profileName, cancellationToken);

            var dependencies =
                _thunderstoreService.ResolveDependenciesAsync(dependency, cancellationToken)
                    .Except(installedMods.Select(mod => mod.FullName).ToAsyncEnumerable(), new DependencyComparer());

            await foreach (var resolvedDependency in dependencies.WithCancellation(cancellationToken))
            {
                await InstallAsync(profileName, resolvedDependency, false, cancellationToken);
            }
        }

        public Task InstallAsync(string profileName, ZipArchive zipArchive, bool skipDependencies, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            throw new NotImplementedException(); // Todo:
        }

        public async Task UninstallAsync(string profileName, ThunderstoreDependency dependency, bool skipDependencies, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var pluginPath = PathHelper.GetBepInExPluginBasePath(profileName);
            pluginPath = Path.Combine(pluginPath, $"{dependency.Name}-{dependency.Author}");

            if (!Directory.Exists(pluginPath))
            {
                return;
            }

            Directory.Delete(pluginPath, true);

            await _profileService.RemoveInstalledModAsync(profileName, dependency, cancellationToken);

            if (skipDependencies)
            {
                return;
            }

            var dependencies = _thunderstoreService.ResolveBackwardDependenciesAsync(dependency, cancellationToken);

            await foreach (var resolvedDependency in dependencies.WithCancellation(cancellationToken))
            {
                await UninstallAsync(profileName, resolvedDependency, true, cancellationToken);
            }
        }
    }
}
