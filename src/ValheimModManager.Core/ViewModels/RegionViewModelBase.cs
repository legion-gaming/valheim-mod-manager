using System;

using Microsoft.Extensions.Logging;

using Prism.Regions;

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
            ITaskAwaiterService taskAwaiterService
        ) : base(logger, taskAwaiterService)
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
        }
    }
}
