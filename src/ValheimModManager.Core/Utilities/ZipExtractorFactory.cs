using System;
using System.IO.Compression;

using ValheimModManager.Core.Helpers;

namespace ValheimModManager.Core.Utilities
{
    public class ZipExtractorFactory
    {
        private readonly ZipArchive _zipArchive;
        private readonly string _basePath;

        public ZipExtractorFactory(ZipArchive zipArchive, string basePath)
        {
            _zipArchive = zipArchive;
            _basePath = basePath;
        }

        public IZipExtractor Create(string dependencyString)
        {
            if (dependencyString.StartsWith("denikson-BepInExPack_Valheim"))
            {
                return new BepInExExtractor(_zipArchive, _basePath);
            }

            if (!DependencyStringHelper.TryParse(dependencyString, out var mod))
            {
                throw new FormatException("Dependency string was not in a valid format.");
            }

            return new ThunderstoreModExtractor(_zipArchive, _basePath, $"{mod.Name}-{mod.Author}");
        }
    }
}
