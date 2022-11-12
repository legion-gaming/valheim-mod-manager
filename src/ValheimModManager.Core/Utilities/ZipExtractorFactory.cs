using System.IO.Compression;

using ValheimModManager.Core.Data;

namespace ValheimModManager.Core.Utilities
{
    public class ZipExtractorFactory
    {
        public IZipExtractor Create(ThunderstoreDependency dependency, ZipArchive zipArchive, string basePath)
        {
            var manifestEntry = zipArchive.GetEntry("manifest.json")!;

            if (dependency.Author.Equals("denikson") && dependency.Name.Equals("BepInExPack_Valheim"))
            {
                return new BepInExExtractor(zipArchive, basePath);
            }

            return new ThunderstoreModExtractor(zipArchive, basePath, $"{dependency.Name}-{dependency.Author}");
        }
    }
}
