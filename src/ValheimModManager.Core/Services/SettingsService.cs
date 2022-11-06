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
        private const string Path = "settings.json";

        private readonly ITaskAwaiterService _taskAwaiterService;
        private readonly Func<Task<IDictionary<string, object>>> _factoryDelegate;

        private AsyncLazy<IDictionary<string, object>> _settings;

        public SettingsService(ITaskAwaiterService taskAwaiterService)
        {
            _taskAwaiterService = taskAwaiterService;
            _factoryDelegate = () => LoadAsync();
            _settings = new AsyncLazy<IDictionary<string, object>>(_factoryDelegate);
        }

        public T Get<T>(string key, T defaultValue = default)
        {
            return _taskAwaiterService.Await(() => GetAsync(key, defaultValue));
        }

        public void Set<T>(string key, T value)
        {
            _taskAwaiterService.Await(SetAsync(key, value));
        }

        public async Task<IDictionary<string, object>> LoadAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!File.Exists(Path))
            {
                return new Dictionary<string, object>();
            }

            using (var settingsFile = File.Open(Path, FileMode.Open, FileAccess.Read))
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

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using
            (
                var fileStream = !File.Exists(Path)
                    ? File.Open(Path, FileMode.CreateNew, FileAccess.Write)
                    : File.Open(Path, FileMode.Truncate, FileAccess.ReadWrite)
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

        private async Task<T> GetAsync<T>(string key, T defaultValue = default, CancellationToken cancellationToken = default)
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

        private async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var settings = await _settings.Value;

            settings[key] = value;
        }
    }
}
