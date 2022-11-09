using System;

using Prism.Regions;

namespace ValheimModManager.Core.ViewModels
{
    public class RegionViewModelBase : ViewModelBase, IConfirmNavigationRequest
    {

        public RegionViewModelBase(IRegionManager regionManager)
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
