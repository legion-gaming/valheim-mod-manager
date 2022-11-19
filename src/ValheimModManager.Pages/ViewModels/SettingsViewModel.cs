using System.Diagnostics;

using Microsoft.Extensions.Logging;

using Prism.Commands;
using Prism.Events;
using Prism.Regions;

using ValheimModManager.Core.Helpers;
using ValheimModManager.Core.Services;
using ValheimModManager.Core.ViewModels;

namespace ValheimModManager.Pages.ViewModels
{
    public class SettingsViewModel : RegionViewModelBase<SettingsViewModel>
    {
        private readonly ISettingsService _settingsService;

        private string _steamLocation;
        private string _additionalSteamArguments;

        public SettingsViewModel
        (
            ILogger<SettingsViewModel> logger,
            IRegionManager regionManager,
            ISettingsService settingsService,
            ITaskAwaiterService taskAwaiterService,
            IEventAggregator eventAggregator
        ) : base(logger, regionManager, taskAwaiterService, eventAggregator)
        {
            _settingsService = settingsService;

            DataFolderCommand = new DelegateCommand(OpenDataFolder);
            SaveCommand = new DelegateCommand(Save);
        }

        public DelegateCommand DataFolderCommand { get; }
        public DelegateCommand SaveCommand { get; }

        public string SteamLocation
        {
            get { return _steamLocation; }
            set { SetProperty(ref _steamLocation, value); }
        }

        public string AdditionalSteamArguments
        {
            get { return _additionalSteamArguments; }
            set { SetProperty(ref _additionalSteamArguments, value); }
        }

        public string DataFolder
        {
            get { return PathHelper.GetBasePath(); }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            SteamLocation = RunAsync(() => _settingsService.GetAsync(nameof(SteamLocation), string.Empty));
            AdditionalSteamArguments = RunAsync(() => _settingsService.GetAsync(nameof(AdditionalSteamArguments), string.Empty));
        }

        private void OpenDataFolder()
        {
            var processStartInfo =
                new ProcessStartInfo
                {
                    FileName = DataFolder + @"\",
                    UseShellExecute = true
                };

            Process.Start(processStartInfo);
        }

        private void Save()
        {
            RunAsync(_settingsService.SetAsync(nameof(SteamLocation), SteamLocation));
            RunAsync(_settingsService.SetAsync(nameof(AdditionalSteamArguments), AdditionalSteamArguments));
        }
    }
}
