using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace ValheimModManager.Core.Utilities
{
    public class ZipExtractor : IZipExtractor
    {
        private readonly ZipArchive _zipArchive;
        private readonly string _basePath;

        public ZipExtractor(ZipArchive zipArchive, string basePath)
        {
            _zipArchive = zipArchive;
            _basePath = basePath;
        }

        public async Task ExtractAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in _zipArchive.Entries)
            {
                if (entry.FullName.EndsWith('/') || entry.FullName.EndsWith('\\'))
                {
                    continue;
                }

                await ExtractEntryAsyncImpl(entry, cancellationToken);
            }
        }

        protected virtual Task ExtractEntryAsync(ZipArchiveEntry entry, CancellationToken cancellationToken = default)
        {
            return SaveEntryToDiskAsync(entry, cancellationToken);
        }

        protected virtual string GetFilePath(ZipArchiveEntry entry)
        {
            return entry.FullName;
        }

        protected virtual bool OverwriteIfExists(ZipArchiveEntry entry)
        {
            return false;
        }

        private Task ExtractEntryAsyncImpl(ZipArchiveEntry entry, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var path = GetFilePathImpl(entry);

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path must have a value.");
            }

            return ExtractEntryAsync(entry, cancellationToken);
        }

        private string GetFilePathImpl(ZipArchiveEntry entry)
        {
            var path = GetFilePath(entry).Replace('/', '\\');
            var basePath = _basePath.Replace('/', '\\');
            var finalPath = Path.Join(basePath, path);

            return finalPath;
        }

        private async Task SaveEntryToDiskAsync(ZipArchiveEntry entry, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var filePath = $"{GetFilePathImpl(entry)}";

            if (File.Exists(filePath) && !OverwriteIfExists(entry))
            {
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!.TrimEnd('\\'));

            using (var entryStream = entry.Open())
            using (var fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                await entryStream.CopyToAsync(fileStream, cancellationToken);
            }
        }
    }
}