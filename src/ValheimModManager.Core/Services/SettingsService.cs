using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using ValheimModManager.Core.Helpers;

namespace ValheimModManager.Core.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly Func<Task<IDictionary<string, object>>> _factoryDelegate;

        private AsyncLazy<IDictionary<string, object>> _settings;

        public SettingsService()
        {
            _factoryDelegate = () => LoadAsync();
            _settings = new AsyncLazy<IDictionary<string, object>>(_factoryDelegate);
        }

        public async Task<T> GetAsync<T>(string key, T defaultValue = default, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var settings = await _settings.Value;

            if (!settings.TryGetValue(key, out var value))
            {
                return defaultValue;
            }

            if (value is JsonElement jsonElement)
            {
                return jsonElement.Deserialize<T>();
            }

            return (T)value;
        }

        public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var settings = await _settings.Value;

            settings[key] = value;

            await SaveAsync(cancellationToken);
        }

        private async Task<IDictionary<string, object>> LoadAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!File.Exists(PathHelper.GetSettingsPath()))
            {
                return new Dictionary<string, object>();
            }

            using (var settingsFile = File.Open(PathHelper.GetSettingsPath(), FileMode.Open, FileAccess.Read))
            {
                var test =
                    await JsonSerializer.DeserializeAsync<IDictionary<string, object>>
                    (
                        settingsFile,
                        cancellationToken: cancellationToken
                    );

                return test;
            }
        }

        private async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using
            (
                var fileStream = !File.Exists(PathHelper.GetSettingsPath())
                    ? File.Open(PathHelper.GetSettingsPath(), FileMode.CreateNew, FileAccess.Write)
                    : File.Open(PathHelper.GetSettingsPath(), FileMode.Truncate, FileAccess.ReadWrite)
            )
            {
                await JsonSerializer.SerializeAsync
                (
                    fileStream,
                    await _settings.Value,
                    cancellationToken: cancellationToken
                );
            }

            _settings = new AsyncLazy<IDictionary<string, object>>(_factoryDelegate);
        }
    }
}
