using System;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace ValheimModManager.Core.Utilities;

public class ThunderstoreModExtractor : ZipExtractor
{
    private readonly string _modName;

    public ThunderstoreModExtractor(ZipArchive zipArchive, string basePath, string modName)
        : base(zipArchive, basePath)
    {
        _modName = modName;
    }

    protected override bool OverwriteIfExists(ZipArchiveEntry entry)
    {
        return !IsConfig(entry);
    }

    protected override string GetFilePath(ZipArchiveEntry entry)
    {
        if (IsDirectory(entry))
        {
            return base.GetFilePath(entry);
        }

        var pathBuilder = new StringBuilder("BepInEx/");

        if (IsConfig(entry))
        {
            pathBuilder.Append("config/");
            pathBuilder.Append(ReplacePathToken(entry.FullName, "config", _modName));

            return pathBuilder.ToString();
        }

        if (IsCore(entry))
        {
            pathBuilder.Append("core/");
            pathBuilder.Append(ReplacePathToken(entry.FullName, "core", _modName));

            return pathBuilder.ToString();
        }

        if (IsPatcher(entry))
        {
            pathBuilder.Append("patchers/");
            pathBuilder.Append(ReplacePathToken(entry.FullName, "patchers", _modName));

            return pathBuilder.ToString();
        }

        if (IsPlugin(entry))
        {
            pathBuilder.Append("plugins/");
            pathBuilder.Append(ReplacePathToken(entry.FullName, "plugins", _modName));

            return pathBuilder.ToString();
        }

        pathBuilder.Append($"plugins/{_modName}/{(!string.IsNullOrWhiteSpace(entry.Name) ? entry.Name : entry.FullName)}");

        return pathBuilder.ToString();
    }

    protected bool IsDirectory(ZipArchiveEntry entry)
    {
        return entry.FullName.EndsWith('/');
    }

    protected bool IsConfig(ZipArchiveEntry entry)
    {
        return !IsDirectory(entry) && entry.FullName.StartsWith("config/");
    }

    protected bool IsCore(ZipArchiveEntry entry)
    {
        return !IsDirectory(entry) && entry.FullName.StartsWith("core/");
    }

    protected bool IsPatcher(ZipArchiveEntry entry)
    {
        return !IsDirectory(entry) && entry.FullName.StartsWith("patchers/");
    }

    protected bool IsPlugin(ZipArchiveEntry entry)
    {
        return !IsDirectory(entry) && entry.FullName.StartsWith("plugins/");
    }

    protected string ReplacePathToken(string path, string token, string newToken)
    {
        var parts =
            path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

        var index = parts.IndexOf(token);

        if (index < 0)
        {
            return path;
        }

        parts.RemoveAt(index);
        parts.Insert(index, newToken);

        return string.Join('/', parts).TrimStart('/').Replace("//", "/");
    }
}