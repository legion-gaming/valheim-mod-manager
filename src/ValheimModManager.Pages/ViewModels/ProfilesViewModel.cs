using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Microsoft.Extensions.Logging;

using Prism.Commands;
using Prism.Events;
using Prism.Regions;

using ValheimModManager.Core.Services;
using ValheimModManager.Core.ViewModels;

namespace ValheimModManager.Pages.ViewModels
{
    public class ProfilesViewModel : RegionViewModelBase<ProfilesViewModel>
    {
        private readonly ISettingsService _settingsService;

        private string _profileName;

        public ProfilesViewModel
        (
            ILogger<ProfilesViewModel> logger,
            IRegionManager regionManager,
            ISettingsService settingsService,
            ITaskAwaiterService taskAwaiterService,
            IEventAggregator eventAggregator
        ) : base(logger, regionManager, taskAwaiterService, eventAggregator)
        {
            _settingsService = settingsService;

            Profiles = new ObservableCollection<string>();

            CreateCommand = new DelegateCommand(Create, CanCreate);
            DeleteCommand = new DelegateCommand(Delete, CanDelete);
        }

        public ObservableCollection<string> Profiles { get; }
        public DelegateCommand CreateCommand { get; }
        public DelegateCommand DeleteCommand { get; }

        public string ProfileName
        {
            get { return _profileName; }
            set { SetProperty(ref _profileName, value); }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            LoadData();
        }

        private void LoadData()
        {
            var profiles = RunAsync(() => _settingsService.GetAsync(nameof(Profiles), new List<string>{"default"}));

            Profiles.Clear();

            foreach (var profile in profiles)
            {
                Profiles.Add(profile);
            }

            CreateCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }

        private void Create()
        {
            Profiles.Add(ProfileName);

            RunAsync(_settingsService.SetAsync(nameof(Profiles), Profiles));

            CreateCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }

        private bool CanCreate()
        {
            var profiles = RunAsync(() => _settingsService.GetAsync(nameof(Profiles), Enumerable.Empty<string>()));

            return !profiles.Contains(ProfileName, StringComparer.OrdinalIgnoreCase);
        }

        private void Delete()
        {
            Profiles.Remove(ProfileName);

            RunAsync(_settingsService.SetAsync(nameof(Profiles), Profiles));

            CreateCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }

        private bool CanDelete()
        {
            return true;
        }
    }
}
