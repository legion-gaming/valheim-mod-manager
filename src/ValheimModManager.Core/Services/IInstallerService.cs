using System.Threading;
using System.Threading.Tasks;

namespace ValheimModManager.Core.Services;

public interface IInstallerService
{
    Task InstallAsync(string profileName, string dependencyString, bool skipDependencies, CancellationToken cancellationToken = default);
    Task UninstallAsync(string profileName, string dependencyString, bool skipDependencies, CancellationToken cancellationToken = default);
}