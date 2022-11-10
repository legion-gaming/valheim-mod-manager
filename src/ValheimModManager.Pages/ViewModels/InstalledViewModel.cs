using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Prism.Commands;
using Prism.Regions;

using ValheimModManager.Core.Data;
using ValheimModManager.Core.Services;
using ValheimModManager.Core.ViewModels;

namespace ValheimModManager.Pages.ViewModels
{
    public class InstalledViewModel : RegionViewModelBase<InstalledViewModel> // Todo: adjust
    {
        private readonly IThunderstoreService _thunderstoreService;
        private readonly IInstallerService _installerService;
        private readonly IProfileService _profileService;
        private readonly ISettingsService _settingsService;

        private int _page = -1;
        private int _pageSize = 10;
        private int _itemCount;
        private string _search;

        public InstalledViewModel
        (
            ILogger<InstalledViewModel> logger,
            IRegionManager regionManager,
            IThunderstoreService thunderstoreService,
            ITaskAwaiterService taskAwaiterService,
            IInstallerService installerService,
            IProfileService profileService,
            ISettingsService settingsService
        ) : base(logger, regionManager, taskAwaiterService)
        {
            _thunderstoreService = thunderstoreService;
            _installerService = installerService;
            _profileService = profileService;
            _settingsService = settingsService;

            Profiles = new ObservableLookup<string, ThunderstoreMod>();

            PreviousCommand = new DelegateCommand(Previous, CanGoPrevious);
            NextCommand = new DelegateCommand(Next, CanGoNext);
            UpdateCommand = new DelegateCommand<ThunderstoreModVersion>(Update); // Todo: change to use ThunderstoreModVersion
            UninstallCommand = new DelegateCommand<ThunderstoreMod>(Uninstall); // Todo: change to use ThunderstoreModVersion
            UninstallWithoutDependenciesCommand = new DelegateCommand<ThunderstoreMod>(UninstallWithoutDependencies); // Todo: change to use ThunderstoreModVersion
        }

        public ObservableLookup<string, ThunderstoreMod> Profiles { get; set; }
        public DelegateCommand PreviousCommand { get; }
        public DelegateCommand NextCommand { get; }
        public DelegateCommand<ThunderstoreModVersion> UpdateCommand { get; }
        public DelegateCommand<ThunderstoreMod> UninstallCommand { get; }
        public DelegateCommand<ThunderstoreMod> UninstallWithoutDependenciesCommand { get; }

        public string SelectedProfile
        {
            get { return _profileService.GetSelectedProfile(); }
            set
            {
                _profileService.SetSelectedProfile(value);
                RaisePropertyChanged();
                RunAsync(LoadDataAsync());
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

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            var profiles = RunAsync(() => _settingsService.GetAsync(nameof(Profiles), new List<string>()));

            foreach (var profile in profiles)
            {
                Profiles.Add(profile, new ObservableCollection<ThunderstoreMod>());
            }

            Page = 1;
        }

        private async Task LoadDataAsync()
        {
            var installedMods = await _thunderstoreService.GetInstalledModsAsync(SelectedProfile);
            var modCache = await _thunderstoreService.GetModsAsync();

            var mods =
                installedMods.Join
                    (
                        modCache,
                        installedMod => $"{installedMod.Author}-{installedMod.Name}",
                        availableMod => $"{availableMod.Owner}-{availableMod.Name}",
                        (_, availableMod) => availableMod
                    )
                    .Where(Filter)
                    .ToList();

            ItemCount = mods.Count;

            Profiles[SelectedProfile].Clear();

            foreach (var mod in mods.Skip((Page - 1) * PageSize).Take(PageSize))
            {
                Profiles[SelectedProfile].Add(mod);
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

        private void Update(ThunderstoreModVersion mod) // Todo:
        {
        }

        private void Uninstall(ThunderstoreMod mod)
        {
            RunAsync(UninstallAsync(mod, false));
        }

        private void UninstallWithoutDependencies(ThunderstoreMod mod)
        {
            RunAsync(UninstallAsync(mod, true));
        }

        private async Task UninstallAsync(ThunderstoreMod mod, bool skipDependencies, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var mods = await _thunderstoreService.GetInstalledModsAsync(SelectedProfile, cancellationToken); // Todo:
            var selectedMod = mods.First(mod.Versions.Contains);

            await _installerService.UninstallAsync(SelectedProfile, selectedMod.FullName, skipDependencies, cancellationToken); // Todo:
            await LoadDataAsync();
        }
    }
}
