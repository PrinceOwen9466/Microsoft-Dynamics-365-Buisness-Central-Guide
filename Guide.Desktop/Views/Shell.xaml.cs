using CommonServiceLocator;
using Guide.Common.Infrastructure;
using Guide.Common.Infrastructure.Resources.Interfaces;
using Prism.Ioc;
using Prism.Logging;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unity;

namespace Guide.Desktop.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell : Window, IShell
    {
        #region Properties

        #region Services
        IRegionManager IRegionManager { get; }
        IUnityContainer Container { get; }
        ILoggerFacade Logger { get; }
        #endregion

        #endregion

        #region Constructors
        public Shell()
        {
            ((App)Application.Current).InitializePrism();
            InitializeComponent();
            ((App)Application.Current).RaiseOnInitialized();


            RoutedEventHandler loaded = null;
            Loaded += loaded = (s, e) =>
            {
                Loaded -= loaded;

                //RegionManager.Regions[Core.MAIN_REGION].Add(Container.Resolve<Views.StartView>(), Core.START_VIEW);
                //RegionManager.Regions[Core.MAIN_REGION].Add(Container.Resolve<Views.MainView>(), Core.MAIN_VIEW);
            };

            Logger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            Container = ServiceLocator.Current.GetInstance<IUnityContainer>();
            IRegionManager = ServiceLocator.Current.GetInstance<IRegionManager>();

            Container.RegisterInstance<IShell>(this);
            DataContext = Container.Resolve<ViewModels.ShellViewModel>();

            Core.Log.Debug("Registering Regions...");
            IRegionManager.Regions[Core.MAIN_REGION].Add(Container.Resolve<Views.StartView>(), Core.START_VIEW);
            IRegionManager.Regions[Core.MAIN_REGION].Add(Container.Resolve<Views.HomeView>(), Core.HOME_VIEW);
            IRegionManager.Regions[Core.MAIN_REGION].Add(Container.Resolve<Views.MainView>(), Core.MAIN_VIEW);
        }
        /*
        public Shell(ILoggerFacade logger, IRegionManager regionManager, IUnityContainer container)
        {
            InitializeComponent();

            Logger = logger;
            Container = container;
            RegionManager = regionManager;

            Container.RegisterInstance<IShell>(this);
            DataContext = Container.Resolve<ViewModels.ShellViewModel>();


            RoutedEventHandler loaded = null;
            Loaded += loaded = (s, e) =>
            {
                Loaded -= loaded;

                return;
                RegionManager.Regions[Core.MAIN_REGION].Add(Container.Resolve<Views.BrowserView>(), "View");
                RegionManager.Regions[Core.MAIN_REGION].Add(Container.Resolve<Views.StartView>(), Core.START_VIEW);
                RegionManager.Regions[Core.MAIN_REGION].Add(Container.Resolve<Views.MainView>(), Core.MAIN_VIEW);
            };

        }
        */
        #endregion
    }
}
