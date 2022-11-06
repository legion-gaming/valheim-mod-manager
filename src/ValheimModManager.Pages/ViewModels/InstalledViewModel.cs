using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Prism.Commands;
using Prism.Regions;

using ValheimModManager.Core.Data;
using ValheimModManager.Core.Services;
using ValheimModManager.Core.ViewModels;

namespace ValheimModManager.Pages.ViewModels
{
    public class InstalledViewModel : RegionViewModelBase // Todo: adjust
    {
        private readonly IThunderstoreService _thunderstoreService;
        private readonly ITaskAwaiterService _taskAwaiterService;
        private readonly IInstallerService _installerService;

        private int _page = -1;
        private int _pageSize = 10;
        private int _itemCount;
        private string _search;

        public InstalledViewModel
        (
            IRegionManager regionManager,
            IThunderstoreService thunderstoreService,
            ITaskAwaiterService taskAwaiterService,
            IInstallerService installerService
        ) : base(regionManager)
        {
            _thunderstoreService = thunderstoreService;
            _taskAwaiterService = taskAwaiterService;
            _installerService = installerService;

            Profiles = new ObservableLookup<string, ThunderstoreMod>("Installed"); // Todo:

            PreviousCommand = new DelegateCommand(Previous, CanGoPrevious);
            NextCommand = new DelegateCommand(Next, CanGoNext);
            UpdateCommand = new DelegateCommand<ThunderstoreModVersion>(Update); // Todo: change to use ThunderstoreModVersion
            UninstallCommand = new DelegateCommand<ThunderstoreMod>(Uninstall); // Todo: change to use ThunderstoreModVersion
            UninstallWithoutDependenciesCommand = new DelegateCommand<ThunderstoreMod>(UninstallWithoutDependencies); // Todo: change to use ThunderstoreModVersion
        }

        public ObservableLookup<string, ThunderstoreMod> Profiles { get; }
        public DelegateCommand PreviousCommand { get; }
        public DelegateCommand NextCommand { get; }
        public DelegateCommand<ThunderstoreModVersion> UpdateCommand { get; }
        public DelegateCommand<ThunderstoreMod> UninstallCommand { get; }
        public DelegateCommand<ThunderstoreMod> UninstallWithoutDependenciesCommand { get; }

        public int Page
        {
            get { return _page; }
            set
            {
                SetProperty(ref _page, value);
                _taskAwaiterService.Await(LoadDataAsync());
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
            Page = 1;
        }

        private async Task LoadDataAsync() // Todo: refactor
        {
            var installedMods = await _thunderstoreService.GetInstalledModsAsync("default");
            var modCache = await _thunderstoreService.GetModsAsync();

            var mods =
                modCache.Join(installedMods, mod => $"{mod.Owner}-{mod.Name}", version => $"{version.Author}-{version.Name}", (mod, _) => mod)
                    .Where(Filter)
                    .ToList(); // Todo:

            ItemCount = mods.Count;

            Profiles["Installed"].Clear();

            foreach (var mod in mods.Skip((Page - 1) * PageSize).Take(PageSize))
            {
                Profiles["Installed"].Add(mod);
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
            _taskAwaiterService.Await(UninstallAsync(mod, false)); // Todo:
        }

        private void UninstallWithoutDependencies(ThunderstoreMod mod)
        {
            _taskAwaiterService.Await(UninstallAsync(mod, true)); // Todo:
        }

        private async Task UninstallAsync(ThunderstoreMod mod, bool skipDependencies, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var mods = await _thunderstoreService.GetInstalledModsAsync("default", cancellationToken); // Todo:
            var selectedMod = mods.First(mod.Versions.Contains);

            await _installerService.UninstallAsync("default", selectedMod.FullName, skipDependencies, cancellationToken); // Todo:
            await LoadDataAsync();
        }
    }
}
