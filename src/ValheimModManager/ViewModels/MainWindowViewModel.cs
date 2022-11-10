using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Prism.Events;

using ValheimModManager.Core.Data;
using ValheimModManager.Core.Services;
using ValheimModManager.Core.ViewModels;

namespace ValheimModManager.ViewModels
{
    public class MainWindowViewModel : ViewModelBase<MainWindowViewModel>
    {
        private string _title = "Valheim Mod Manager";
        private bool _isDownloading;

        public MainWindowViewModel
        (
            ILogger<MainWindowViewModel> logger,
            IEventAggregator eventAggregator,
            ITaskAwaiterService taskAwaiterService
        ) : base(logger, taskAwaiterService)
        {
            eventAggregator.GetEvent<TaskStatusEvent>()
                .Subscribe(SetIsDownloading, ThreadOption.UIThread);
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

        private void SetIsDownloading(Task task)
        {
            IsDownloading = !task.IsCompleted;
        }
    }
}
