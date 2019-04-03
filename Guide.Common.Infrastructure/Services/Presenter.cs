using Guide.Common.Infrastructure.Models;
using Guide.Common.Infrastructure.Models.Interfaces;
using Guide.Common.Infrastructure.Resources.Controls;
using Guide.Common.Infrastructure.Resources.Interfaces;
using Guide.Common.Infrastructure.Services.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Guide.Common.Infrastructure.Services
{
    public class Presenter : BindableBase, IPresenter
    {
        #region Properties
        IPresentable _content = null;
        public IPresentable Content { get => _content; private set => SetProperty(ref _content, value); }

        IPlayer _player = null;
        public IPlayer Player { get => _player; set => SetProperty(ref _player, value); }

        // public bool FullPageActive => Content != null && Content.View != null && Content.View.IsFullPage;
        bool _fullPageActive = false;
        public bool FullPageActive { get => _fullPageActive; set => SetProperty(ref _fullPageActive, value); }

        bool _videoActive = false;
        public bool VideoActive { get => _videoActive; set => SetProperty(ref _videoActive, value); }


        IView _currentView = null;
        public IView CurrentView { get => _currentView; set => SetProperty(ref _currentView, value); }

        Direction _swipeDirection = Direction.Right;
        public Direction SwipeDirection
        {
            get => _swipeDirection;
            set => SetProperty(ref _swipeDirection, value);
        }

        bool _isHome = false;
        public bool IsHome { get => _isHome; set => SetProperty(ref _isHome, value); }

        public event EventHandler Initialized;
        #region Services
        IUnityContainer Container { get; }
        #endregion

        #region Internals
        IView HomeView { get; set; }
        
        #endregion

        #endregion

        #region Constructors
        public Presenter(IUnityContainer container)
        {
            Container = container;
            Core.Log.Debug("A new presenter has been constructed");   
        }
        #endregion

        #region Methods

        public void Initialize(IPresentable content, IView home)
        {
            HomeView = home;
            NavigateHome();

            Content = content;

            if (Content is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)Content).PropertyChanged += (s, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case nameof(Content.View):
                            if (!IsHome)
                                CurrentView = Content.View;
                            break;
                    }
                };
            }

            Initialized?.Invoke(this, EventArgs.Empty);
        }

        public async void Next()
        {
            FullPageActive = false;
            if (IsHome && Content != null)
            {
                CurrentView = Content.View;
                IsHome = false;
                RaisePropertyChanged(nameof(FullPageActive));
                return;
            }

            IsHome = false;
            await Content.Next();
            RaisePropertyChanged(nameof(FullPageActive));
        }

        public async void Previous()
        {
            FullPageActive = false;
            IsHome = false;
            await Content.Previous();
            RaisePropertyChanged(nameof(FullPageActive));
        }

        public void RefreshFullView() => RaisePropertyChanged(nameof(FullPageActive));

        public void NavigateHome()
        {
            FullPageActive = false;
            IsHome = true;
            SwipeDirection = Direction.Down;
            if (Content != null) Content.NavigateTo(0, false);
            CurrentView = HomeView;
        }

        public async void NavigateToSection(Section section)
        {
            IsHome = false;
            SwipeDirection = Direction.Down;
            await Content.OpenSection(section);
        }
        
        public async void NavigateToPage(Page page)
        {
            FullPageActive = false;
            IsHome = false;
            SwipeDirection = Direction.Down;
            await Content.NavigateTo(page);
        }
        #endregion
    }
}
