using System.Windows;

using Prism.Ioc;
using Prism.Modularity;

using ValheimModManager.Pages;
using ValheimModManager.UI;
using ValheimModManager.Views;

namespace ValheimModManager
{
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<PagesModule>();
            moduleCatalog.AddModule<UIModule>();
        }
    }
}
