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

        Task<ThunderstoreModVersion> GetModAsync(string dependencyString,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ThunderstoreModVersion>> GetInstalledModsAsync(string profileName,
            CancellationToken cancellationToken = default);

        Task<ZipArchive> DownloadModAsync(string url, CancellationToken cancellationToken = default);
    }
}
