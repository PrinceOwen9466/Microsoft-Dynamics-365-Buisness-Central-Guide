using Guide.Common.Infrastructure;
using Guide.Common.Infrastructure.Resources.Controls;
using Guide.ContentLibrary.Views;
using Guide.ContentLibrary.Views.Introduction;
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
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : PresentationControl
    {
        #region Properties

        #region Services
        ILoggerFacade Logger { get; }
        IRegionManager RegionManager { get; }
        IUnityContainer Container { get; }
        #endregion

        public NavigationControl NavControl => NavigationControl;

        #endregion


        public MainView(ILoggerFacade logger, IRegionManager regionManager, IUnityContainer container)
        {
            InitializeComponent();

            Logger = logger;
            RegionManager = regionManager;
            Container = container;
            RoutedEventHandler loaded = null;
            Loaded += loaded = (s, e) =>
            {
                Loaded -= loaded;
                //RegionManager.Regions[Core.CONTENT_REGION].Add(Container.Resolve<WelcomeToBusinessCentral>(), "Sample Summary View");

                
            };
        }
    }
}
