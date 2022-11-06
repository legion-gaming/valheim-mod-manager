using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace ValheimModManager.Core.Services
{
    public interface ISettingsService // Todo:
    {
        T Get<T>(string key, T defaultValue = default);
        void Set<T>(string key, T value);

        Task<IDictionary<string, object>> LoadAsync(CancellationToken cancellationToken = default);
        Task SaveAsync(CancellationToken cancellationToken = default);
    }
}