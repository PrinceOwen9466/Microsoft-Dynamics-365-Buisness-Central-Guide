using CommonServiceLocator;
using Gecko;
using Prism.Ioc;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Guide.Demo.GeckoWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        public App()
        {
            Gecko.Xpcom.Initialize("Firefox");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //base.OnStartup(e);
            
        }

        protected override Window CreateShell()
        {

            return MainWindow;
            
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //base.RegisterTypes(containerRegistry);
            //containerRegistry.RegisterSingleton<ILoggerFacade, NLogger>();
            //containerRegistry.RegisterSingleton<IPresenter, Presenter>();
        }
    }
}
