using System.Collections.Generic;
using System.IO.Compression;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ValheimModManager.Core.Data;

namespace ValheimModManager.Core.Services
{
    public class ThunderstoreClient
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _options;

        public ThunderstoreClient()
        {
            _client = new HttpClient();

            _options =
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
        }

        public async Task<IReadOnlyList<ThunderstoreMod>> GetModsAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var url = "https://valheim.thunderstore.io/api/v1/package/";

            var response =
                await _client.GetStreamAsync(url, cancellationToken);

            using (response)
            {
                return
                    await JsonSerializer.DeserializeAsync<IReadOnlyList<ThunderstoreMod>>(response, _options, cancellationToken);
            }
        }

        public async Task<ZipArchive> DownloadModAsync(string url, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _client.GetStreamAsync(url, cancellationToken);

            var response = await _client.GetAsync(url, cancellationToken);

            var responseStream =
                await response.EnsureSuccessStatusCode()
                    .Content
                    .ReadAsStreamAsync(cancellationToken);

            return new ZipArchive(responseStream, ZipArchiveMode.Read);
        }
    }
}
