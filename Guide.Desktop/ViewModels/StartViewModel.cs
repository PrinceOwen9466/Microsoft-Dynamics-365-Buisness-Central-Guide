using Guide.Common.Infrastructure;
using Guide.Common.Infrastructure.Extensions;
using Guide.Common.Infrastructure.Resources.Interfaces;
using Guide.Common.Infrastructure.Services.Interfaces;
using Prism.Commands;
using Prism.Logging;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Guide.Desktop.ViewModels
{
    public class StartViewModel : IViewAware
    {
        #region Properties
        #region Statics
        const double AnimationDuration = 5.6;
        #endregion

        IView _view = null;
        public IView View
        {
            get => _view;
            set
            {
                _view = value;
                ActivateView();
            }
        }

        #region Services
        ILoggerFacade Logger { get; }
        IRegionManager RegionManager { get; }
        #endregion

        #region Commands
        public ICommand ViewLoadedCommand { get; }
        public ICommand MoveCommand { get; }
        #endregion

        #region Internals
        DispatcherTimer Timer { get; }
        IShell Shell { get; }
        bool ViewActive => View != null;
        #endregion

        #endregion

        #region Constructors
        public StartViewModel(ILoggerFacade logger, IRegionManager regionManager, IShell shell)
        {
            Logger = logger;
            RegionManager = regionManager;
            Shell = shell;

            ViewLoadedCommand = new DelegateCommand(OnViewLoaded);
            MoveCommand = new DelegateCommand(OnMove);
            /*
            Timer = new DispatcherTimer(TimeSpan.FromSeconds(AnimationDuration), DispatcherPriority.Normal, new EventHandler((s, e) =>
            {
                OnAnimationComplete();
            }), Dispatcher.CurrentDispatcher);
            Timer.IsEnabled = false;
            */
            

        }
        #endregion

        #region Methods

        #region Command Handlers
        
        void OnViewLoaded()
        {
            //Timer.Start();
        }

        void OnAnimationComplete()
        {
            //Timer.Stop();
            //RegionManager.RequestNavigateToView(Core.MAIN_REGION, Core.MAIN_VIEW);
            Logger.Debug("Guide.Desktop has been successfully initialized!!!");
        }

        void OnMove()
        {
            try { Shell.DragMove(); }
            catch { }
        }


        #endregion

        void ActivateView()
        {
            if (!ViewActive) return;
            View.Opened += (s, e) =>
            {
                RegionManager.RequestNavigateToView(Core.MAIN_REGION, Core.MAIN_VIEW);
                Logger.Debug("Guide.Desktop has been successfully initialized!!!");
            };
        }

        #endregion
    }
}
