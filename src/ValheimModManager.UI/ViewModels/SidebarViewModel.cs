using System;
using System.Diagnostics;
using System.IO;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

using ValheimModManager.Core.Data;
using ValheimModManager.Core.Services;

namespace ValheimModManager.UI.ViewModels
{
    public class SidebarViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly ISettingsService _settingsService;

        public SidebarViewModel(IRegionManager regionManager, ISettingsService settingsService)
        {
            _regionManager = regionManager;
            _settingsService = settingsService;

            StartModdedCommand = new DelegateCommand(StartModded);
            StartVanillaCommand = new DelegateCommand(StartVanilla);
            NavigateCommand = new DelegateCommand<string>(Navigate, CanNavigate);
        }

        public DelegateCommand StartModdedCommand { get; }
        public DelegateCommand StartVanillaCommand { get; }
        public DelegateCommand<string> NavigateCommand { get; }

        public string SteamLocation
        {
            get { return _settingsService.Get(nameof(SteamLocation), string.Empty); }
        }

        public string AdditionalSteamArguments
        {
            get { return _settingsService.Get(nameof(AdditionalSteamArguments), string.Empty); }
        }

        private void Navigate(string path)
        {
            _regionManager.RequestNavigate(RegionName.Page, path);
        }

        private bool CanNavigate(string path)
        {
            return true; // Todo:
        }

        private void StartModded() // Todo:
        {
            var currentDirectory = Environment.CurrentDirectory;
            var currentPath = Path.Join(currentDirectory, "profiles\\default\\BepInEx\\core\\BepInEx.Preloader.dll");

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
