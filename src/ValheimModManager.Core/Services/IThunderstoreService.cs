using System.Collections.Generic;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

using ValheimModManager.Core.Data;

namespace ValheimModManager.Core.Services
{
    public interface IThunderstoreService
    {
        Task<IReadOnlyList<ThunderstoreMod>> GetModsAsync(CancellationToken cancellationToken = default);
        Task<ThunderstoreModVersion> GetModAsync(ThunderstoreDependency dependency, CancellationToken cancellationToken = default);
        Task<ZipArchive> DownloadModAsync(ThunderstoreDependency dependency, CancellationToken cancellationToken = default);
        IAsyncEnumerable<string> ResolveDependenciesAsync(ThunderstoreDependency dependency, CancellationToken cancellationToken = default);
        IAsyncEnumerable<string> ResolveBackwardDependenciesAsync(ThunderstoreDependency dependency, CancellationToken cancellationToken = default);
    }
}
