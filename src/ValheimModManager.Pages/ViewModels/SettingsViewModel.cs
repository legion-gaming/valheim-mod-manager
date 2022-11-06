using System.Runtime.CompilerServices;

using Prism.Commands;
using Prism.Regions;

using ValheimModManager.Core.Services;
using ValheimModManager.Core.ViewModels;

namespace ValheimModManager.Pages.ViewModels
{
    public class SettingsViewModel : RegionViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly ITaskAwaiterService _taskAwaiterService;

        public SettingsViewModel
        (
            IRegionManager regionManager,
            ISettingsService settingsService,
            ITaskAwaiterService taskAwaiterService
        ) : base(regionManager)
        {
            _settingsService = settingsService;
            _taskAwaiterService = taskAwaiterService;

            SaveCommand = new DelegateCommand(Save);
        }

        public DelegateCommand SaveCommand { get; }

        public string SteamLocation
        {
            get { return Get(defaultValue: ""); }
            set { Set(value); }
        }

        public string AdditionalSteamArguments
        {
            get { return Get(defaultValue: string.Empty); }
            set { Set(value); }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            RaisePropertyChanged(nameof(SteamLocation));
            RaisePropertyChanged(nameof(AdditionalSteamArguments));
        }

        private void Save()
        {
            _taskAwaiterService.Await(_settingsService.SaveAsync());
        }

        private T Get<T>([CallerMemberName] string propertyName = null, T defaultValue = default)
        {
            return _settingsService.Get(propertyName, defaultValue);
        }

        private void Set<T>(T value, [CallerMemberName] string propertyName = null)
        {
            _settingsService.Set(propertyName, value);
            RaisePropertyChanged(propertyName);
        }
    }
}