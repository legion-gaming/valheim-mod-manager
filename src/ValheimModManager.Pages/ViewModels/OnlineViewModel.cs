using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Prism.Commands;
using Prism.Events;
using Prism.Regions;

using ValheimModManager.Core.Data;
using ValheimModManager.Core.Services;
using ValheimModManager.Core.ViewModels;

namespace ValheimModManager.Pages.ViewModels
{
    public class OnlineViewModel : RegionViewModelBase<OnlineViewModel>
    {
        private readonly IThunderstoreService _thunderstoreService;
        private readonly IInstallerService _installerService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IProfileService _profileService;
        private readonly ISettingsService _settingsService;

        private int _page = -1;
        private int _pageSize = 10;
        private int _itemCount;
        private string _search;
        private bool _canDownloadMod = true;

        public OnlineViewModel
        (
            ILogger<OnlineViewModel> logger,
            IRegionManager regionManager,
            IThunderstoreService thunderstoreService,
            IInstallerService installerService,
            ITaskAwaiterService taskAwaiterService,
            IEventAggregator eventAggregator,
            IProfileService profileService,
            ISettingsService settingsService
        ) : base(logger, regionManager, taskAwaiterService)
        {
            _thunderstoreService = thunderstoreService;
            _installerService = installerService;
            _eventAggregator = eventAggregator;
            _profileService = profileService;
            _settingsService = settingsService;

            Profiles = new ObservableLookup<string, ThunderstoreMod>(); // Todo:

            PreviousCommand = new DelegateCommand(Previous, CanGoPrevious);
            NextCommand = new DelegateCommand(Next, CanGoNext);
            DownloadCommand = new DelegateCommand<ThunderstoreModVersion>(Download, CanDownload);
            DownloadWithoutDependenciesCommand = new DelegateCommand<ThunderstoreModVersion>(DownloadWithoutDependencies, CanDownload);
        }

        public ObservableLookup<string, ThunderstoreMod> Profiles { get; }
        public DelegateCommand PreviousCommand { get; }
        public DelegateCommand NextCommand { get; }
        public DelegateCommand<ThunderstoreModVersion> DownloadCommand { get; }
        public DelegateCommand<ThunderstoreModVersion> DownloadWithoutDependenciesCommand { get; }

        public string SelectedProfile
        {
            get { return _profileService.GetSelectedProfile(); }
            set
            {
                _profileService.SetSelectedProfile(value);
                RaisePropertyChanged();
            }
        }

        public int Page
        {
            get { return _page; }
            set
            {
                SetProperty(ref _page, value);
                RunAsync(LoadDataAsync());
                PreviousCommand.RaiseCanExecuteChanged();
                NextCommand.RaiseCanExecuteChanged();
            }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                SetProperty(ref _pageSize, value);
                RaisePropertyChanged(nameof(PageCount));

                Page = 1;
            }
        }

        public int ItemCount
        {
            get { return _itemCount; }
            set
            {
                SetProperty(ref _itemCount, value);
                RaisePropertyChanged(nameof(PageCount));
            }
        }

        public int PageCount
        {
            get { return (int)Math.Ceiling((double)ItemCount / PageSize); }
        }

        public string Search
        {
            get { return _search; }
            set
            {
                SetProperty(ref _search, value);
                Page = 1;
            }
        }

        public bool CanDownloadMod
        {
            get { return _canDownloadMod; }
            set { SetProperty(ref _canDownloadMod, value); }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<TaskStatusEvent>()
                .Subscribe
                (
                    status =>
            {
                CanDownloadMod = status.IsCompleted;
                DownloadCommand.RaiseCanExecuteChanged();
                    }
                );

            Page = 1;
        }

        private async Task LoadDataAsync()
        {
            var modCache = await _thunderstoreService.GetModsAsync();
            var mods = modCache.Where(Filter).ToList();

            ItemCount = mods.Count;

            var profiles =
                RunAsync(() => _settingsService.GetAsync(nameof(Profiles), new List<string>()));

            Profiles.Clear();

            foreach (var profile in profiles)
            {
                Profiles.Add(profile, new ObservableCollection<ThunderstoreMod>());

                foreach (var mod in mods.Skip((Page - 1) * PageSize).Take(PageSize))
                {
                    Profiles[profile].Add(mod);
                }
            }

            PreviousCommand.RaiseCanExecuteChanged();
            NextCommand.RaiseCanExecuteChanged();
        }

        private bool Filter(ThunderstoreMod mod)
        {
            if (string.IsNullOrWhiteSpace(Search))
            {
                return true;
            }

            var comparison = StringComparison.OrdinalIgnoreCase;
            var result = true;

            result &= mod.Name.Contains(Search, comparison);
            result |= mod.Owner.StartsWith(Search, comparison);
            result |= mod.Latest.Description.Contains(Search);

            return result;
        }

        private void Previous()
        {
            --Page;
        }

        private bool CanGoPrevious()
        {
            return Page > 1;
        }

        private void Next()
        {
            ++Page;
        }

        private bool CanGoNext()
        {
            return Page < PageCount;
        }

        private void Download(ThunderstoreModVersion mod)
        {
            RunAsync(_installerService.InstallAsync(SelectedProfile, mod.FullName, false), notifyStatus: true);
        }

        private bool CanDownload(ThunderstoreModVersion mod)
        {
            return CanDownloadMod;
        }

        private void DownloadWithoutDependencies(ThunderstoreModVersion mod)
        {
            RunAsync(_installerService.InstallAsync(SelectedProfile, mod.FullName, true), notifyStatus: true);
        }
    }
}
