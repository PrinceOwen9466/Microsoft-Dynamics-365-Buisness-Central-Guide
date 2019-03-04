using Guide.Common.Infrastructure.Models;
using Guide.Common.Infrastructure.Services.Interfaces;
using Guide.ContentLibrary;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guide.Desktop.ViewModels
{
    public class HomeViewModel : BindableBase, INavigationAware
    {
        #region Properties

        #region Services
        ILoggerFacade Logger { get; }
        public IConfigurationManager ConfigurationManager { get; }
        IPresenter Presenter { get; set; }
        #endregion

        #region Bindables
        public Configuration Configuration { get; set; }
        #endregion

        #endregion

        #region Constructors
        public HomeViewModel(ILoggerFacade logger, IConfigurationManager configurationManager, IPresenter presenter)
        {
            Logger = logger;
            ConfigurationManager = configurationManager;
            Presenter = presenter;

            /*
            ContentCore.Initialize();
            Presenter.Initialize(ContentCore.Container);*/
        }
        #endregion

        #region Methods

        #region INavigationAware Implementation
        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext) { }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Configuration = new Configuration(ConfigurationManager.CurrentConfiguration);
            RaisePropertyChanged(nameof(Configuration));
        }
        #endregion

        #endregion
    }
}
