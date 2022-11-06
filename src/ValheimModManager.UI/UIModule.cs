using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using ValheimModManager.Core.Data;
using ValheimModManager.UI.Views;

namespace ValheimModManager.UI
{
    public class UIModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public UIModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion(RegionName.Sidebar, typeof(Sidebar));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}