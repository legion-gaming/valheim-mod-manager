using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

using ValheimModManager.Core.Data;
using ValheimModManager.Core.Services;
using ValheimModManager.Pages.Views;

namespace ValheimModManager.Pages
{
    public class PagesModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public PagesModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionName.Page, nameof(Installed));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ThunderstoreClient>();
            containerRegistry.RegisterSingleton<IThunderstoreService, ThunderstoreService>();
            containerRegistry.RegisterSingleton<ITaskAwaiterService, TaskAwaiterService>();
            containerRegistry.RegisterSingleton<IInstallerService, InstallerService>();
            containerRegistry.RegisterSingleton<ISettingsService, SettingsService>();
            containerRegistry.RegisterSingleton<IProfileService, ProfileService>();

            containerRegistry.RegisterForNavigation<Installed>();
            containerRegistry.RegisterForNavigation<Online>();
            containerRegistry.RegisterForNavigation<Profiles>();
            containerRegistry.RegisterForNavigation<Settings>();
        }
    }
}
