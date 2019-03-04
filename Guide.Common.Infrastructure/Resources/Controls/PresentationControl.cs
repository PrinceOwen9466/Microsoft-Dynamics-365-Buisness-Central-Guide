﻿using CommonServiceLocator;
using Guide.Common.Infrastructure.Models.Interfaces;
using Guide.Common.Infrastructure.Services.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    public enum ControlStatus { Idle, Open, Closed }

    public class PresentationControl : UserControl, IView
    {
        #region Properties

        #region Bindables
        public int Index { get; set; }
        public ISection Section { get; set; }
        public ControlStatus Status { get; set; }

        public bool IsOpened { get; set; }
        int OpenCount { get; set; }

        #region Dependency Properties


        public string Key
        {
            get { return (string)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(string), typeof(PresentationControl), new PropertyMetadata(string.Empty));



        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(PresentationControl), new UIPropertyMetadata(string.Empty, (s, e) =>
            {
                if (!(s is PresentationControl)) return;

                var control = (PresentationControl)s;
                if (string.IsNullOrWhiteSpace(control.Key))
                    control.Key = ((string)e.NewValue).Replace(" ", "");
            }));



        public bool IsFullPage
        {
            get { return (bool)GetValue(IsFullPageProperty); }
            set { SetValue(IsFullPageProperty, value); }
        }
       
        public static readonly DependencyProperty IsFullPageProperty =
            DependencyProperty.Register("IsFullPage", typeof(bool), typeof(PresentationControl), new UIPropertyMetadata(false));

        public bool AutoOpen
        {
            get { return (bool)GetValue(AutoOpenProperty); }
            set { SetValue(AutoOpenProperty, value); }
        }
        public static readonly DependencyProperty AutoOpenProperty =
            DependencyProperty.Register("AutoOpen", typeof(bool), typeof(PresentationControl), new UIPropertyMetadata(false));



        public bool OpenAnimationActive
        {
            get { return (bool)GetValue(OpenAnimationActiveProperty); }
            set { SetValue(OpenAnimationActiveProperty, value); }
        }
        public static readonly DependencyProperty OpenAnimationActiveProperty =
            DependencyProperty.Register("OpenAnimationActive", typeof(bool), typeof(PresentationControl), new UIPropertyMetadata(false));



        public bool IsAnimatable
        {
            get { return (bool)GetValue(IsAnimatableProperty); }
            set { SetValue(IsAnimatableProperty, value); }
        }

        public static readonly DependencyProperty IsAnimatableProperty =
            DependencyProperty.Register("IsAnimatable", typeof(bool), typeof(PresentationControl), new UIPropertyMetadata(true));



        /// <summary>
        /// States how many times the storyboard completed will be called
        /// before this control is really open.
        /// </summary>
        public int CompletedCount
        {
            get { return (int)GetValue(CompletedCountProperty); }
            set { SetValue(CompletedCountProperty, value); }
        }
        public static readonly DependencyProperty CompletedCountProperty =
            DependencyProperty.Register("CompletedCount", typeof(int), typeof(PresentationControl), new PropertyMetadata(1));


        #endregion

        #endregion

        #region Events
        public event EventHandler Opening;
        public event EventHandler Closing;

        public event EventHandler Opened;
        public event EventHandler Closed;
        #endregion

        #region Internals
        IPresenter Presenter { get; }
        #endregion

        #endregion

        #region Constructors
        public PresentationControl()
        {
            //Presenter = ServiceLocator.Current.TryResolve<IPresenter>();
            RoutedEventHandler loaded = null;
            Loaded += loaded = (s, e) =>
            {
                Loaded -= loaded;
                if (DataContext is IViewAware) ((IViewAware)DataContext).View = this;

                if (AutoOpen)
                    Open(); 
            };

            IsVisibleChanged += (s, e) =>
            {
                if (!IsVisible) return;
                //Presenter.
            };
        }
        #endregion

        #region Methods
        public void Open()
        {
            Opening?.Invoke(this, EventArgs.Empty);
            Core.Log.Debug($"Opening Presentation Control {AutoOpen}");
            if (!OpenAnimationActive) OnOpen();
            /*
            if (!OpenDuration.HasTimeSpan) Opened?.Invoke(this, EventArgs.Empty);
            else
            {
                Timer.Interval = OpenDuration.TimeSpan;

                EventHandler tick = null;
                Timer.Tick += tick = (s, e) =>
                {
                    Timer.Tick -= tick;
                    Timer.Stop();

                    Opened?.Invoke(this, EventArgs.Empty);
                };
            }
            */
        }

        public void OnOpen()
        {
            OpenCount++;
            if (OpenCount < CompletedCount) return;
            Core.Log.Debug("Presentation Control Opened Event thrown, {0}", this);
            IsOpened = true;
            Opened?.Invoke(this, EventArgs.Empty);
        }

        public void OnClose()
        {
            Core.Log.Debug("Presentation Control Closed");
            Closed?.Invoke(this, EventArgs.Empty);
        }

        public void Close()
        {
            Closing?.Invoke(this, EventArgs.Empty);

            /*
            if (!CloseDuration.HasTimeSpan) Closed?.Invoke(this, EventArgs.Empty);

            
            EventHandler tick = null;
            Timer.Tick += tick = (s, e) =>
            {
                Timer.Tick -= tick;
                Timer.Stop();

                Closed?.Invoke(this, EventArgs.Empty);
            };

            */
        }
        #endregion
    }
}