using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ValheimModManager.Core.Data;

namespace ValheimModManager.Core.Services
{
    public interface IProfileService
    {
        Task<IList<ThunderstoreModVersion>> GetInstalledModsAsync(string profileName, CancellationToken cancellationToken = default);
        Task AddInstalledModAsync(string profileName, ThunderstoreDependency dependency, CancellationToken cancellationToken = default);
        Task RemoveInstalledModAsync(string profileName, ThunderstoreDependency dependency, CancellationToken cancellationToken = default);

        string GetSelectedProfile();
        void SetSelectedProfile(string profileName);
    }
}
