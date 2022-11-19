using System;
using System.IO;

namespace ValheimModManager.Core.Helpers
{
    public static class PathHelper
    {
        public static string GetProfileBasePath(string profileName)
        {
            var basePath = GetBasePath();

            return Path.Combine(basePath, "profiles", profileName);
        }

        public static string GetProfilePath(string profileName)
        {
            var basePath = GetProfileBasePath(profileName);

            return Path.Combine(basePath, "profile.json");
        }

        public static string GetBepInExBasePath(string profileName)
        {
            var basePath = GetProfileBasePath(profileName);

            return Path.Combine(basePath, "BepInEx");
        }

        public static string GetBepInExConfigBasePath(string profileName)
        {
            var basePath = GetBepInExBasePath(profileName);

            return Path.Combine(basePath, "config");
        }

        public static string GetBepInExCoreBasePath(string profileName)
        {
            var basePath = GetBepInExBasePath(profileName);

            return Path.Combine(basePath, "core");
        }

        public static string GetBepInExPatcherBasePath(string profileName)
        {
            var basePath = GetBepInExBasePath(profileName);

            return Path.Combine(basePath, "patchers");
        }

        public static string GetBepInExPluginBasePath(string profileName)
        {
            var basePath = GetBepInExBasePath(profileName);

            return Path.Combine(basePath, "plugins");
        }

        public static string GetSettingsPath()
        {
            var basePath = GetBasePath();

            return Path.Combine(basePath, "settings.json");
        }

        public static string GetBasePath()
        {
            #if DEBUG

            var basePath = Environment.CurrentDirectory;

            #else

            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            #endif

            return Path.Combine(basePath, "ValheimModManager");
        }
    }
}
