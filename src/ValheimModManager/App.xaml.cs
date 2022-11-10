using System.Windows;

using Microsoft.Extensions.Logging;

using NLog.Extensions.Logging;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;

using Unity;
using Unity.Microsoft.Logging;

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

        protected override IContainerExtension CreateContainerExtension()
        {
            var container = new UnityContainer();
            var loggerFactory = LoggerFactory.Create(builder => builder.AddNLog("nlog.config"));

            container.AddExtension(new LoggingExtension(loggerFactory));

            return new UnityContainerExtension(container);
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
