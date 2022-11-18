using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using ValheimModManager.Core.Comparers;
using ValheimModManager.Core.Data;
using ValheimModManager.Core.Helpers;

namespace ValheimModManager.Core.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IThunderstoreService _thunderstoreService;
        private readonly ISettingsService _settingsService;

        private string _selectedProfile;

        public ProfileService
        (
            IThunderstoreService thunderstoreService,
            ISettingsService settingsService
        )
        {
            _thunderstoreService = thunderstoreService;
            _settingsService = settingsService;
        }

        public Task<List<string>> GetProfilesAsync(CancellationToken cancellationToken = default)
        {
            return _settingsService.GetAsync("Profiles", new List<string>(), cancellationToken);
        }

        public async Task<IList<ThunderstoreModVersion>> GetInstalledModsAsync(string profileName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var profilePath = PathHelper.GetProfilePath(profileName);

            if (!File.Exists(profilePath))
            {
                return new List<ThunderstoreModVersion>();
            }

            IList<string> installedMods;
            using (var profileStream = File.Open(profilePath, FileMode.Open, FileAccess.Read))
            {
                installedMods =
                    await JsonSerializer.DeserializeAsync<IList<string>>
                    (
                        profileStream,
                        cancellationToken: cancellationToken
                    );
            }

            var onlineMods = await _thunderstoreService.GetModsAsync(cancellationToken);

            return
                installedMods!.Join
                    (
                        onlineMods.SelectMany(mod => mod.Versions),
                        installed => installed,
                        online => online.FullName,
                        (_, online) => online
                    )
                    .ToList();
        }

        public async Task AddInstalledModAsync(string profileName, ThunderstoreDependency dependency, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var profilePath = PathHelper.GetProfilePath(profileName);

            if (!File.Exists(profilePath))
            {
                using (var profileStream = File.Open(profilePath!, FileMode.Create, FileAccess.Write))
                {
                    await JsonSerializer.SerializeAsync
                    (
                        profileStream,
                        new string[] { dependency },
                        cancellationToken: cancellationToken
                    );
                }
            }
            else
            {
                IList<string> installedMods;
                using (var profileStream = File.Open(profilePath, FileMode.Open, FileAccess.Read))
                {
                    installedMods =
                        await JsonSerializer.DeserializeAsync<IList<string>>
                        (
                            profileStream,
                            cancellationToken: cancellationToken
                        );
                }

                var comparer = new DependencyComparer();
                var hashSet = new HashSet<string>(installedMods!, comparer);

                if (!hashSet.Add(dependency))
                {
                    hashSet.TryGetValue(dependency, out var existingDependency);

                    if (comparer.Compare(dependency, existingDependency) > 0)
                    {
                        hashSet.Remove(existingDependency);
                        hashSet.Add(dependency);
                    }
                }

                using (var profileStream = File.Open(profilePath, FileMode.Truncate, FileAccess.Write))
                {
                    await JsonSerializer.SerializeAsync
                    (
                        profileStream,
                        hashSet,
                        cancellationToken: cancellationToken
                    );
                }
            }
        }

        public async Task RemoveInstalledModAsync(string profileName, ThunderstoreDependency dependency, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var profilePath = PathHelper.GetProfilePath(profileName);

            if (!File.Exists(profilePath))
            {
                return;
            }

            IList<string> installedMods;
            using (var profileStream = File.Open(profilePath, FileMode.Open, FileAccess.Read))
            {
                installedMods =
                    await JsonSerializer.DeserializeAsync<IList<string>>
                    (
                        profileStream,
                        cancellationToken: cancellationToken
                    );
            }

            var comparer = new DependencyComparer();
            var hashSet = new HashSet<string>(installedMods!, comparer);

            if (!hashSet.Remove(dependency))
            {
                return;
            }

            if (dependency.Author == "denikson" && dependency.Name == "BepInExPack_Valheim")
            {
                hashSet.Clear();
            }

            using (var profileStream = File.Open(profilePath, FileMode.Truncate, FileAccess.Write))
            {
                await JsonSerializer.SerializeAsync
                (
                    profileStream,
                    hashSet,
                    cancellationToken: cancellationToken
                );
            }
        }

        public string GetSelectedProfile()
        {
            return _selectedProfile;
        }

        public void SetSelectedProfile(string profileName)
        {
            _selectedProfile = profileName;
        }
    }
}
