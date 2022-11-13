using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Prism.Commands;
using Prism.Events;
using Prism.Regions;

using ValheimModManager.Core.Data;
using ValheimModManager.Core.Services;
using ValheimModManager.Core.ViewModels;
using ValheimModManager.Pages.Models;

namespace ValheimModManager.Pages.ViewModels
{
    public class InstalledViewModel : RegionViewModelBase<InstalledViewModel>
    {
        private readonly IThunderstoreService _thunderstoreService;
        private readonly IProfileService _profileService;
        private readonly IInstallerService _installerService;
        private readonly ISettingsService _settingsService;

        private int _page = -1;
        private int _pageSize = 10;
        private string _sort = "Last Updated";
        private int _itemCount;
        private string _search;
        private bool _canUninstallMod = true;

        public InstalledViewModel
        (
            ILogger<InstalledViewModel> logger,
            IRegionManager regionManager,
            IThunderstoreService thunderstoreService,
            IProfileService profileService,
            ITaskAwaiterService taskAwaiterService,
            IInstallerService installerService,
            IEventAggregator eventAggregator,
            ISettingsService settingsService
        ) : base(logger, regionManager, taskAwaiterService, eventAggregator)
        {
            _thunderstoreService = thunderstoreService;
            _profileService = profileService;
            _installerService = installerService;
            _settingsService = settingsService;

            Profiles = new ObservableLookup<string, Mod>();

            PreviousCommand = new DelegateCommand(Previous, CanGoPrevious);
            NextCommand = new DelegateCommand(Next, CanGoNext);
            UpdateCommand = new DelegateCommand<ThunderstoreModVersion>(Update);
            UninstallCommand = new DelegateCommand<Mod>(Uninstall, _ => CanUninstallMod);
            UninstallWithoutDependenciesCommand = new DelegateCommand<Mod>(UninstallWithoutDependencies, _ => CanUninstallMod);
        }

        public ObservableLookup<string, Mod> Profiles { get; }
        public DelegateCommand PreviousCommand { get; }
        public DelegateCommand NextCommand { get; }
        public DelegateCommand<ThunderstoreModVersion> UpdateCommand { get; }
        public DelegateCommand<Mod> UninstallCommand { get; }
        public DelegateCommand<Mod> UninstallWithoutDependenciesCommand { get; }

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

        public string Sort
        {
            get { return _sort; }
            set
            {
                SetProperty(ref _sort, value);
                RunAsync(LoadDataAsync());
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

        public bool CanUninstallMod
        {
            get { return _canUninstallMod; }
            set { SetProperty(ref _canUninstallMod, value); }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            var profiles = RunAsync(() => _settingsService.GetAsync(nameof(Profiles), new List<string>()));

            Profiles.Clear();

            foreach (var profile in profiles)
            {
                Profiles.Add(profile, new ObservableCollection<Mod>());
            }

            Page = 1;
        }

        public override void CanExecuteTaskChanged(Task task)
        {
            CanUninstallMod = task.IsCompleted;

            UninstallCommand.RaiseCanExecuteChanged();
            UninstallWithoutDependenciesCommand.RaiseCanExecuteChanged();
        }

        private async Task LoadDataAsync()
        {
            var installedMods = await _profileService.GetInstalledModsAsync(SelectedProfile);
            var modCache = await _thunderstoreService.GetModsAsync();

            var mods =
                SortResults
                    (
                        installedMods.Join
                            (
                                modCache,
                                installedMod => $"{installedMod.Author}-{installedMod.Name}",
                                onlineMod => $"{onlineMod.Owner}-{onlineMod.Name}",
                                (installedMod, onlineMod) =>
                                {
                                    var mod = (Mod)installedMod;

                                    mod.Versions =
                                        onlineMod.Versions
                                            .Where
                                            (
                                                version =>
                                                    Version.Parse(version.VersionNumber) >=
                                                    Version.Parse(installedMod.VersionNumber)
                                            );

                                    return mod;
                                }
                            )
                            .Where(Filter)
                    )
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

        private bool Filter(Mod mod)
        {
            if (string.IsNullOrWhiteSpace(Search))
            {
                return true;
            }

            var comparison = StringComparison.OrdinalIgnoreCase;
            var result = true;

            result &= mod.Name.Contains(Search, comparison);
            result |= mod.Author.StartsWith(Search, comparison);
            result |= mod.Description.Contains(Search);

            return result;
        }

        private IEnumerable<Mod> SortResults(IEnumerable<Mod> mods)
        {
            switch (Sort.ToLower().Replace(" ", string.Empty))
            {
                default:
                    return mods;

                case "lastupdated":
                    return mods.OrderByDescending(mod => mod.LastUpdated);

                case "modname":
                    return mods.OrderBy(mod => mod.Name);

                case "authorname":
                    return mods.OrderBy(mod => mod.Author);
            }
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

        private void Update(ThunderstoreModVersion mod)
        {
            RunAsync(UpdateAsync(mod), notifyStatus: true);
        }

        private void Uninstall(Mod mod)
        {
            RunAsync(UninstallAsync(mod, false), notifyStatus: true);
        }

        private void UninstallWithoutDependencies(Mod mod)
        {
            RunAsync(UninstallAsync(mod, true), notifyStatus: true);
        }

        private async Task UpdateAsync(ThunderstoreModVersion mod, CancellationToken cancellationToken = default)
        {
            await _installerService.InstallAsync(SelectedProfile, mod.FullName, true, cancellationToken);
            await LoadDataAsync();
        }

        private async Task UninstallAsync(Mod mod, bool skipDependencies, CancellationToken cancellationToken = default)
        {
            await _installerService.UninstallAsync(SelectedProfile, mod.Dependency, skipDependencies, cancellationToken);
            await LoadDataAsync();
        }
    }
}
