using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using Prism.Commands;
using Prism.Regions;

using ValheimModManager.Core.Services;
using ValheimModManager.Core.ViewModels;

namespace ValheimModManager.Pages.ViewModels
{
    public class ProfilesViewModel : RegionViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly ITaskAwaiterService _taskAwaiterService;

        private string _profileName;

        public ProfilesViewModel
        (
            IRegionManager regionManager,
            ISettingsService settingsService,
            ITaskAwaiterService taskAwaiterService
        ) : base(regionManager)
        {
            _settingsService = settingsService;
            _taskAwaiterService = taskAwaiterService;

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
            var profiles = _settingsService.Get(nameof(Profiles), new List<string>{"default"});

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

            _settingsService.Set(nameof(Profiles), Profiles);
            _taskAwaiterService.Await(_settingsService.SaveAsync());

            CreateCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }

        private bool CanCreate()
        {
            var profiles = _settingsService.Get(nameof(Profiles), Enumerable.Empty<string>());

            return !profiles.Contains(ProfileName, StringComparer.OrdinalIgnoreCase);
        }

        private void Delete()
        {
            Profiles.Remove(ProfileName);

            _settingsService.Set(nameof(Profiles), Profiles);
            _taskAwaiterService.Await(_settingsService.SaveAsync());

            CreateCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }

        private bool CanDelete()
        {
            return true;
        }
    }
}
