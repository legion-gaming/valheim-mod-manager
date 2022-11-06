using System.Threading;
using System.Threading.Tasks;

namespace ValheimModManager.Core.Utilities;

public interface IZipExtractor
{
    Task ExtractAsync(CancellationToken cancellationToken = default);
}