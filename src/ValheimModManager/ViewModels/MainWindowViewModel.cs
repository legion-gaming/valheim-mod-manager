using Prism.Events;
using Prism.Mvvm;

using ValheimModManager.Core.Data;

namespace ValheimModManager.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Valheim Mod Manager";
        private bool _isDownloading;

        public MainWindowViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<TaskStatusEvent>().Subscribe(status => IsDownloading = !status.IsCompleted);
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public bool IsDownloading
        {
            get { return _isDownloading; }
            set { SetProperty(ref _isDownloading, value); }
        }
    }
}
