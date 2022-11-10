using System.Diagnostics;
using System.IO;

using Microsoft.Extensions.Logging;

using Prism.Commands;
using Prism.Regions;

using ValheimModManager.Core.Data;
using ValheimModManager.Core.Helpers;
using ValheimModManager.Core.Services;
using ValheimModManager.Core.ViewModels;

namespace ValheimModManager.UI.ViewModels
{
    public class SidebarViewModel : RegionViewModelBase<SidebarViewModel>
    {
        private readonly ISettingsService _settingsService;
        private readonly IProfileService _profileService;

        public SidebarViewModel
        (
            ILogger<SidebarViewModel> logger,
            IRegionManager regionManager,
            ISettingsService settingsService,
            ITaskAwaiterService taskAwaiterService,
            IProfileService profileService
        ) : base(logger, regionManager, taskAwaiterService)
        {
            _settingsService = settingsService;
            _profileService = profileService;

            StartModdedCommand = new DelegateCommand(StartModded);
            StartVanillaCommand = new DelegateCommand(StartVanilla);
            NavigateCommand = new DelegateCommand<string>(Navigate, CanNavigate);
        }

        public DelegateCommand StartModdedCommand { get; }
        public DelegateCommand StartVanillaCommand { get; }
        public DelegateCommand<string> NavigateCommand { get; }

        public string SteamLocation
        {
            get { return RunAsync(() => _settingsService.GetAsync(nameof(SteamLocation), string.Empty)); }
        }

        public string AdditionalSteamArguments
        {
            get { return RunAsync(() => _settingsService.GetAsync(nameof(AdditionalSteamArguments), string.Empty)); }
        }

        private void Navigate(string path)
        {
            RegionManager.RequestNavigate(RegionName.Page, path);
        }

        private bool CanNavigate(string path)
        {
            return true; // Todo:
        }

        private void StartModded() // Todo:
        {
            var currentDirectory = PathHelper.GetBepInExCoreBasePath(_profileService.GetSelectedProfile());
            var currentPath = Path.Combine(currentDirectory, "BepInEx.Preloader.dll");

            var arguments =
                new[]
                {
                    AdditionalSteamArguments,
                    "-applaunch 892970",
                    "--doorstop-enable true",
                    "--doorstop-target",
                    currentPath
                };

            Process.Start(SteamLocation, string.Join(" ", arguments));
        }

        private void StartVanilla() // Todo:
        {
            var arguments =
                new[]
                {
                    AdditionalSteamArguments,
                    "-applaunch 892970"
                };

            Process.Start(SteamLocation, string.Join(" ", arguments));
        }
    }
}
