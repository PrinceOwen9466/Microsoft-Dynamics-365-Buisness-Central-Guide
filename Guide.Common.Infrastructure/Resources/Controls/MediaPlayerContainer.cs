using Guide.Common.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    public class MediaPlayerContainer : Control
    {
        #region Properties

        #region Dependency Properties


        public string ResourcePath
        {
            get { return (string)GetValue(ResourcePathProperty); }
            set { SetValue(ResourcePathProperty, value); }
        }
        public static readonly DependencyProperty ResourcePathProperty =
            DependencyProperty.Register("ResourcePath", typeof(string), typeof(MediaPlayerContainer), new UIPropertyMetadata(string.Empty));


        public Assembly ResourceAssembly
        {
            get { return (Assembly)GetValue(ResourceAssemblyProperty); }
            set { SetValue(ResourceAssemblyProperty, value); }
        }
        public static readonly DependencyProperty ResourceAssemblyProperty =
            DependencyProperty.Register("ResourceAssembly", typeof(Assembly), typeof(MediaPlayerContainer), new UIPropertyMetadata(null));


        public double SpeedRatio
        {
            get { return (double)GetValue(SpeedRatioProperty); }
            set { SetValue(SpeedRatioProperty, value); }
        }

        public static readonly DependencyProperty SpeedRatioProperty =
            DependencyProperty.Register("SpeedRatio", typeof(double), typeof(MediaPlayerContainer), new UIPropertyMetadata(1.0));




        public bool IsPlaying
        {
            get { return (bool)GetValue(IsPlayingProperty); }
            private set
            {
                SetValue(IsPlayingProperty, value);

                if (WaitControl == null) return;
                if (!value) WaitControl.Visibility = Visibility.Visible;
                else WaitControl.Visibility = Visibility.Hidden;
            }
        }

        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register("IsPlaying", typeof(bool), typeof(MediaPlayerContainer), new UIPropertyMetadata(false));



        #endregion

        #region Internals

        #region Elements
        MediaElement Player { get; set; }
        UserControl WaitControl { get; set; }
        Button Clickable { get; set; }
        #endregion

        Uri Source { get; set; }
        DispatcherTimer Timer { get; set; }
        #endregion

        #endregion

        #region Constructors
        static MediaPlayerContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MediaPlayerContainer),
                new FrameworkPropertyMetadata(typeof(MediaPlayerContainer)));
        }

        public MediaPlayerContainer()
        {
            // Loaded += (s, e) => Source = new Uri(ResourceHelper.CreateLocalInstance(ResourcePath, ResourceAssembly), UriKind.Absolute);
            IsVisibleChanged += (s, e) =>
            {
                if (ResourcePath == null || ResourceAssembly == null) return;
                if (IsVisible) Start();
                else Stop();
            };
            Timer = new DispatcherTimer(DispatcherPriority.Normal, Dispatcher.CurrentDispatcher) { Interval = TimeSpan.FromSeconds(2) };
        }
        #endregion

        #region Methods
        
        async void Start()
        {
            try
            {
                await ResolveResource();
                Player.Source = Source;
                Player.Play();
                IsPlaying = true;
            }
            catch { }
            
        }

        void Stop()
        {
            Player.Stop();
            IsPlaying = false;

            Timer.Tick += async (s, e) =>
            {
                Timer.Stop();
                await DissolveResource();
            };
            Timer.Start();
        }

        Task ResolveResource()
        {
            Source = new Uri(ResourceHelper.CreateLocalInstance(ResourcePath, ResourceAssembly), UriKind.Absolute);
            return Task.CompletedTask;  
        }

        Task DissolveResource()
        {
            if (IsPlaying) return Task.CompletedTask;
            try
            {
                Player.Stop();
                Player.ClearValue(MediaElement.SourceProperty);
                ResourceHelper.DissolveLocalInstance(Source.OriginalString);
            }
            catch
            {
                if (Source == null) return Task.CompletedTask;
                if (File.Exists(Source.OriginalString))
                    try { File.Delete(Source.OriginalString); }
                    catch { Timer.Start(); }
            }
            return Task.CompletedTask;
        }

        #region Overrides
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


            Player = GetTemplateChild("PART_Player") as MediaElement;
            Player.SpeedRatio = SpeedRatio;
            
            Player.MediaEnded += (s, e) => Player.Position = new TimeSpan(0, 0, 1);

            WaitControl = GetTemplateChild("PART_WaitControl") as UserControl;

            Player.PreviewMouseLeftButtonDown += (s, e) =>
            {
                RippleControl control = Player.FindParent<RippleControl>();
                if (control == null) return;

                control.Play(Player);
            };
        }
        #endregion

        #endregion
    }
}
