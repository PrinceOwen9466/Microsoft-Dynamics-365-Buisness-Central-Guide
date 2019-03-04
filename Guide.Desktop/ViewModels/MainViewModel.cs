//using CefSharp.Wpf;
using Guide.Common.Infrastructure;
using Guide.Common.Infrastructure.Extensions;
using Guide.Common.Infrastructure.Models;
using Guide.Common.Infrastructure.Resources.Controls;
using Guide.Common.Infrastructure.Resources.Interfaces;
using Guide.Common.Infrastructure.Services.Interfaces;
using Guide.ContentLibrary;
using Guide.Desktop.Views;
using Prism.Commands;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Unity;

namespace Guide.Desktop.ViewModels
{
    public class MainViewModel : BindableBase, IViewAware
    {
        #region Properties

        #region Statics
        const double AnimationDuration = 0.6;
        #endregion

        IView _view = null;
        public IView View
        {
            get => _view;
            set
            {
                _view = value;
                /*
                if (_view != null && _view.IsOpened) OnOpened();
                else if (_view != null) _view.Opened += (s, e) => OnOpened();
                */
            }
        }

        #region Services
        ILoggerFacade Logger { get; }
        IRegionManager RegionManager { get; }
        IUnityContainer Container { get; }
        public IPresenter Presenter { get; }
        #endregion

        #region Events
        public event EventHandler SlideBarOpen;
        public event EventHandler SlideBarClosed;

        public event EventHandler TitleBarDocked;
        public event EventHandler TitleBarUndocked;
        #endregion

        #region Bindables
        bool _slideBarActive = false;
        public bool SlideBarActive
        {
            get => _slideBarActive;
            set
            {
                bool changed = _slideBarActive != value;
                SetProperty(ref _slideBarActive, value);

                if (changed && value) SlideBarOpen?.Invoke(this, EventArgs.Empty);
                else if (changed && !value) SlideBarClosed?.Invoke(this, EventArgs.Empty);
            }
        }

        bool _titleBarActive = true;
        public bool TitleBarActive
        {
            get => _titleBarActive;
            set
            {
                SetProperty(ref _titleBarActive, value);
            }
        }

        bool _titleBarPinned = true;
        public bool TitleBarPinned
        {
            get => _titleBarPinned;
            set
            {
                bool changed = _titleBarPinned != value;
                SetProperty(ref _titleBarPinned, value);

                if (changed)
                {
                    if (value) TitleBarDocked?.Invoke(this, EventArgs.Empty);
                    else TitleBarUndocked?.Invoke(this, EventArgs.Empty);
                }
                SearchActive = false;
            }
        }

        public bool FullPageActive => Presenter != null && Presenter.FullPageActive;

        bool _searchActive = false;
        public bool SearchActive
        {
            get => _searchActive;
            set => SetProperty(ref _searchActive, value);
        }


        //public Page CurrentPage { get; }
        #endregion

        #region Commands
        public ICommand OpenSlideBarCommand { get; }
        public ICommand CloseSlideBarCommand { get; }
        public ICommand MinimizeCommand { get; }
        public ICommand MoveCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand MouseMoveTitleBarCommand { get; }
        public ICommand MouseLeaveTitleBarCommand { get; }

        public ICommand PreviousCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand SidebarNavigateCommand { get; }
        public ICommand HomeCommand { get; set; }
        #endregion

        #region Internals
        IShell Shell { get; }
        #endregion

        #endregion

        #region Constructors
        public MainViewModel(ILoggerFacade logger, IRegionManager regionManager, IPresenter presenter, IUnityContainer container, IShell shell)
        {
            RegionManager = regionManager;
            Presenter = presenter;
            Logger = logger;
            Shell = shell;
            Container = container;

            OpenSlideBarCommand = new DelegateCommand(OnOpenSlideBar);
            CloseSlideBarCommand = new DelegateCommand(OnCloseSlideBar);
            MinimizeCommand = new DelegateCommand(OnMinimize);
            MoveCommand = new DelegateCommand(OnMove);
            CloseCommand = new DelegateCommand(OnClose);

            MouseMoveTitleBarCommand = new DelegateCommand(OnMouseEnterTitleBar);
            MouseLeaveTitleBarCommand = new DelegateCommand(OnMouseLeaveTitleBar);


            PreviousCommand = new DelegateCommand(OnPrevious);
            NextCommand = new DelegateCommand(OnNext);

            SidebarNavigateCommand = new DelegateCommand<object>(OnSidebarNavigate);
        }
        #endregion

        #region Methods

        #region Command Handlers

        void OnCloseSlideBar()
        {
            SlideBarActive = false;                 
        }

        void OnOpenSlideBar()
        {
            SlideBarActive = !SlideBarActive;
        }

        void OnMove()
        {
            try { Shell.DragMove(); }
            catch { }
        }

        void OnMinimize() => Shell.WindowState = System.Windows.WindowState.Minimized;

        void OnClose() => Shell.Close();

        void OnMouseLeaveTitleBar()
        {
            if (!TitleBarPinned) TitleBarActive = false;
        }

        void OnMouseEnterTitleBar()
        {
            if (FullPageActive) return;

            if (!TitleBarActive)
                TitleBarActive = true;
        }

        void OnOpened()
        {
            /*
            ContentCore.Initialize();
            Presenter.Initialize(ContentCore.Container, Container.Resolve<HomeView>());
            */
        }

        void OnPrevious()
        {
            Presenter.SwipeDirection = Direction.Left;
            SlideBarActive = false;
            Presenter.Previous();

            //if (FullPageActive) TitleBarPinned = false;
            RaisePropertyChanged(nameof(Presenter));
        }

        void OnNext()
        {
            Presenter.SwipeDirection = Direction.Right;
            SlideBarActive = false; 
            Presenter.Next();

            //if (FullPageActive) TitleBarPinned = false;
            RaisePropertyChanged(nameof(Presenter));
        }

        void OnSidebarNavigate(object obj)
        {
            if (!(obj is Section)) return;

            Presenter.SwipeDirection = Direction.Down;
            Presenter.Content.OpenSection((Section)obj);
            SlideBarActive = false;
            RaisePropertyChanged(nameof(FullPageActive));
        }

        void OnGoHome()
        {
            
        }
        #endregion

        #endregion
    }
}
