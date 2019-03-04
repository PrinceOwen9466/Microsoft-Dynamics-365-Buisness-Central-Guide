using Guide.Common.Infrastructure.Models;
using Guide.Common.Infrastructure.Models.Interfaces;
using Guide.Common.Infrastructure.Resources.Controls;
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
        public bool FullPageActive => Content != null && Content.View != null && Content.View.IsFullPage;

        IView _currentView = null;
        public IView CurrentView { get => _currentView; set => SetProperty(ref _currentView, value); }

        Direction _swipeDirection = Direction.Right;
        public Direction SwipeDirection
        {
            get => _swipeDirection;
            set => SetProperty(ref _swipeDirection, value);
        }
        #region Services
        IUnityContainer Container { get; }
        #endregion

        #region Internals
        IView HomeView { get; set; }
        bool IsHome { get; set; }
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
            // IsHome = true;
            CurrentView = HomeView = home;
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
        }

        public async void Next()
        {
            await Content.Next();
            //CurrentView = Content.View;
            //RaisePropertyChanged(nameof(Content));
            //RaisePropertyChanged(nameof(FullPageActive));
        }

        public async void Previous()
        {
            await Content.Previous();
            CurrentView = Content.View;
            RaisePropertyChanged(nameof(FullPageActive));
        }

        public void RefreshFullView() => RaisePropertyChanged(nameof(FullPageActive));
        #endregion
    }
}
