using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Prism.Events;
using Prism.Regions;

using ValheimModManager.Core.Data;
using ValheimModManager.Core.Services;

namespace ValheimModManager.Core.ViewModels
{
    public class RegionViewModelBase<TViewModel> : ViewModelBase<TViewModel>, IConfirmNavigationRequest
        where TViewModel : ViewModelBase<TViewModel>
    {
        public RegionViewModelBase
        (
            ILogger<TViewModel> logger,
            IRegionManager regionManager,
            ITaskAwaiterService taskAwaiterService,
            IEventAggregator eventAggregator
        ) : base(logger, taskAwaiterService, eventAggregator)
        {
            RegionManager = regionManager;
        }

        protected IRegionManager RegionManager { get; }

        public virtual void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            continuationCallback(true);
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
            EventAggregator.GetEvent<TaskStatusEvent>()
                .Subscribe(CanExecuteTaskChanged);
        }

        public virtual void CanExecuteTaskChanged(Task task)
        {
        }
    }
}
