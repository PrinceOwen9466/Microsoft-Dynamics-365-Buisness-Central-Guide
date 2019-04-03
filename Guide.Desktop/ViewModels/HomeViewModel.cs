using Guide.Common.Infrastructure.Models;
using Guide.Common.Infrastructure.Services.Interfaces;
using Guide.ContentLibrary;
using Prism.Commands;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Guide.Desktop.ViewModels
{
    public class HomeViewModel : BindableBase, INavigationAware
    {
        #region Properties

        #region Services
        ILoggerFacade Logger { get; }
        public IConfigurationManager ConfigurationManager { get; }
        public IPresenter Presenter { get; set; }
        #endregion

        #region Bindables
        bool _particleActive = true;
        public bool ParticlesActive { get => _particleActive; set => SetProperty(ref _particleActive, value); }
        public Configuration Configuration { get; set; }
        #endregion

        #region Commands
        public ICommand ResumeCommand { get; }
        #endregion

        #endregion

        #region Constructors
        public HomeViewModel(ILoggerFacade logger, IConfigurationManager configurationManager, IPresenter presenter)
        {
            Logger = logger;
            ConfigurationManager = configurationManager;
            Presenter = presenter;

            ResumeCommand = new DelegateCommand(OnResume);
            /*
            ContentCore.Initialize();
            Presenter.Initialize(ContentCore.Container);*/
        }
        #endregion

        #region Methods

        #region INavigationAware Implementation
        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext) { ParticlesActive = false; }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            ParticlesActive = true;
            Configuration = new Configuration(ConfigurationManager.CurrentConfiguration);
            RaisePropertyChanged(nameof(Configuration));
        }
        #endregion

        void OnResume()
        {
            if (ConfigurationManager.CurrentConfiguration?.RecentPage == null) return;
            Presenter.NavigateToPage(ConfigurationManager.CurrentConfiguration.RecentPage);
        }


        #endregion
    }
}
