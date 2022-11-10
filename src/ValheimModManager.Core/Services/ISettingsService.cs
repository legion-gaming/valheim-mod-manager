using System.Threading.Tasks;
using System.Threading;

namespace ValheimModManager.Core.Services
{
    public interface ISettingsService
    {
        Task<T> GetAsync<T>(string key, T defaultValue = default, CancellationToken cancellationToken = default);
        Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);
    }
}
