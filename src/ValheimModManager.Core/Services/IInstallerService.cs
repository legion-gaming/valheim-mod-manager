using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

using ValheimModManager.Core.Data;

namespace ValheimModManager.Core.Services
{
    public interface IInstallerService
    {
        Task InstallAsync(string profileName, ThunderstoreDependency dependency, bool skipDependencies, CancellationToken cancellationToken = default);
        Task InstallAsync(string profileName, ZipArchive zipArchive, bool skipDependencies, CancellationToken cancellationToken = default);
        Task UninstallAsync(string profileName, ThunderstoreDependency dependency, bool skipDependencies, CancellationToken cancellationToken = default);
    }
}
