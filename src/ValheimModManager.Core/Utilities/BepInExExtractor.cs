using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ValheimModManager.Core.Utilities
{
    public class BepInExExtractor : ThunderstoreModExtractor
    {
        public BepInExExtractor(ZipArchive zipArchive, string basePath)
            : base(zipArchive, basePath, "BepInEx")
        {
        }

        protected override bool OverwriteIfExists(ZipArchiveEntry entry)
        {
            return true;
        }

        protected override async Task ExtractEntryAsync(ZipArchiveEntry entry, CancellationToken cancellationToken = default)
        {
            var ignored =
                new[]
                {
                    "BepInExPack_Valheim/",
                    "icon.png",
                    "manifest.json",
                    "README.md"
                };

            if (ignored.Contains(entry.FullName))
            {
                return;
            }

            await base.ExtractEntryAsync(entry, cancellationToken);
        }

        protected override string GetFilePath(ZipArchiveEntry entry)
        {
            return ReplacePathToken(entry.FullName, "BepInExPack_Valheim", string.Empty);
        }
    }
}