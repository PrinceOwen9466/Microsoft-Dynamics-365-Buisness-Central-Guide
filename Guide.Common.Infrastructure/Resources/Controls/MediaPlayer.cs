using Guide.Common.Infrastructure.Extensions;
using Guide.Common.Infrastructure.Resources.Containers;
using Guide.Common.Infrastructure.Resources.Interfaces;
using Prism.Commands;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    public class MediaPlayer : Control, INotifyPropertyChanged, IPlayer
    {
        #region Properties

        SynchronizationContext Context { get; } = SynchronizationContext.Current;

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler BackInvoked;
        public event EventHandler Activated;
        #endregion

        #region Statics
        static double PLAY_INTERVAL = 100;
        static double JUMP_LENGTH = 10;
        #endregion

        #region Dependency Properties
        public bool IsFull
        {
            get { return (bool)GetValue(IsFullProperty); }
            set { SetValue(IsFullProperty, value); }
        }
        public static readonly DependencyProperty IsFullProperty =
            DependencyProperty.Register("IsFull", typeof(bool), typeof(MediaPlayer), new UIPropertyMetadata(false));

        public MediaSource Media
        {
            get { return (MediaSource)GetValue(MediaProperty); }
            set { SetValue(MediaProperty, value); }
        }
        public static readonly DependencyProperty MediaProperty =
            DependencyProperty.Register("Media", typeof(MediaSource), typeof(MediaPlayer), new UIPropertyMetadata(null, (s, e) =>
            {
                if (!(s is MediaPlayer)) return;
                if (!(e.NewValue is MediaSource)) return;

                MediaSource mediaSource = (MediaSource)e.NewValue;
                MediaPlayer player = (MediaPlayer)s;
                player.Stop(false);
                player.DataContext = mediaSource.Context;

                if (e.OldValue is MediaSource) ((MediaSource)e.OldValue).ContextChanged -= player.ContextChanged;
                mediaSource.ContextChanged += player.ContextChanged;

                if (!player.Active || !player.Player.IsLoaded) return;
                try
                {
                    player.Pause();
                } 
                catch (Exception ex) { Core.Log.Debug($"An error occured while pausing media\n {ex}\n{player} --- {player.Player.LoadedBehavior}"); }
                
                player.Player.Position = TimeSpan.FromTicks(1);
            }));

        public double SpeedRatio
        {
            get { return (double)GetValue(SpeedRatioProperty); }
            set { SetValue(SpeedRatioProperty, value); }
        }
        public static readonly DependencyProperty SpeedRatioProperty =
            DependencyProperty.Register("SpeedRatio", typeof(double), typeof(MediaPlayer), new UIPropertyMetadata(1.0, (s, e) =>
            {
                Core.Log.Debug($"New Speed Ratio {e.NewValue}");
                if (!(s is MediaPlayer)) return;
                if (!(e.NewValue is double)) return;

                Core.Log.Debug("New speed ratio value is double");
                MediaPlayer player = (MediaPlayer)s;
                double ratio = (double)e.NewValue;

                if (!player.Active) return;
                if (player.SpeedDragActive) return;
                player.Player.SpeedRatio = ratio;
                Core.Log.Debug($"The speed ratio was set to {player.Player.SpeedRatio}");
            }));



        public double MediaPosition
        {
            get
            {
                if (Active) return Player.Position.TotalSeconds;
                return (double)GetValue(MediaPositionProperty);
            }
            set { SetValue(MediaPositionProperty, value); }
        }
        public static readonly DependencyProperty MediaPositionProperty =
            DependencyProperty.Register("MediaPosition", typeof(double), typeof(MediaPlayer), new FrameworkPropertyMetadata(.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (s, e) =>
            {
                if (!(s is MediaPlayer)) return;
                if (!(e.NewValue is double)) return;

                MediaPlayer player = (MediaPlayer)s;
                double position = (double)e.NewValue;

                if (!player.Active) return;
            }));




        public double MediaDuration
        {
            get { return (double)GetValue(MediaDurationProperty); }
            set { SetValue(MediaDurationProperty, value); }
        }
        public static readonly DependencyProperty MediaDurationProperty =
            DependencyProperty.Register("MediaDuration", typeof(double), typeof(MediaPlayer), new PropertyMetadata(.0));

        public TimeSpan HoverPosition
        {
            get { return (TimeSpan)GetValue(HoverPositionProperty); }
            private set { SetValue(HoverPositionProperty, value); }
        }

        public static readonly DependencyProperty HoverPositionProperty =
            DependencyProperty.Register("HoverPosition", typeof(TimeSpan), typeof(MediaPlayer), new PropertyMetadata(TimeSpan.FromSeconds(0)));

        public double HoverPoint
        {
            get { return (double)GetValue(HoverPointProperty); }
            private set { SetValue(HoverPointProperty, value); }
        }
        public static readonly DependencyProperty HoverPointProperty =
            DependencyProperty.Register("HoverPoint", typeof(double), typeof(MediaPlayer), new PropertyMetadata(.0));



        public MediaPlacemark HoverPlacemark
        {
            get { return (MediaPlacemark)GetValue(HoverPlacemarkProperty); }
            set { SetValue(HoverPlacemarkProperty, value); }
        }

        public static readonly DependencyProperty HoverPlacemarkProperty =
            DependencyProperty.Register("HoverPlacemark", typeof(MediaPlacemark), typeof(MediaPlayer), new UIPropertyMetadata(null));



        public double SpeedHoverPosition
        {
            get { return (double)GetValue(SpeedHoverPositionProperty); }
            set { SetValue(SpeedHoverPositionProperty, value); }
        }
        public static readonly DependencyProperty SpeedHoverPositionProperty =
            DependencyProperty.Register("SpeedHoverPosition", typeof(double), typeof(MediaPlayer), new PropertyMetadata(.0));



        public double SpeedHoverPoint
        {
            get { return (double)GetValue(SpeedHoverPointProperty); }
            set { SetValue(SpeedHoverPointProperty, value); }
        }
        public static readonly DependencyProperty SpeedHoverPointProperty =
            DependencyProperty.Register("SpeedHoverPoint", typeof(double), typeof(MediaPlayer), new PropertyMetadata(.0));

        public bool IsPlaying
        {
            get { return (bool)GetValue(IsPlayingProperty); }
            set { SetValue(IsPlayingProperty, value); }
        }
        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register("IsPlaying", typeof(bool), typeof(MediaPlayer), new UIPropertyMetadata(false));

        public bool Loop
        {
            get { return (bool)GetValue(LoopProperty); }
            set { SetValue(LoopProperty, value); }
        }
        public static readonly DependencyProperty LoopProperty =
            DependencyProperty.Register("Loop", typeof(bool), typeof(MediaPlayer), new PropertyMetadata(true));



        public MediaPlacemark CurrentPlacemark
        {
            get { return (MediaPlacemark)GetValue(CurrentPlacemarkProperty); }
            set { SetValue(CurrentPlacemarkProperty, value); }
        }

        public static readonly DependencyProperty CurrentPlacemarkProperty =
            DependencyProperty.Register("CurrentPlacemark", typeof(MediaPlacemark), typeof(MediaPlayer), new UIPropertyMetadata(null));


        #endregion

        #region Commands
        public ICommand JumpCommand { get; }
        #endregion

        #region Internals

        bool _active = false;
        bool Active
        {
            get => _active;
            set
            {
                _active = value;
                if (value) Activated?.Invoke(this, EventArgs.Empty);
            }
        }
        bool DragActive { get; set; }
        bool SpeedDragActive { get; set; }
        Timer PlayTimer { get; set; }

        TimeSpan _internalPosition = TimeSpan.MinValue;
        TimeSpan InternalPosition
        {
            get => _internalPosition;
            set
            {
                _internalPosition = value;


                if (_internalPosition != null & !DragActive)
                    MediaPosition = _internalPosition.TotalSeconds;
            }
        }
        #region Elements
        MediaElement Player { get; set; }
        UserControl WaitControl { get; set; }

        #region Buttons
        Button PlayButton { get; set; }
        Button JumpForward { get; set; }
        Button JumpBackward { get; set; }
        Button IdeaButton { get; set; }
        Button SpeedButton { get; set; }
        Button BackButton { get; set; }
        #endregion

        #region Sliders
        Panel SliderPanel { get; set; }
        Slider Slider { get; set; }

        Panel SpeedSliderPanel { get; set; }
        Slider SpeedSlider { get; set; }
        #endregion

        #region Title Panel
        Panel TitlePanel { get; set; }
        #endregion

        #endregion

        #region Event Handlers
        RoutedEventHandler MediaEndedHandler;
        EventHandler<MediaContext> ContextChanged;
        #endregion

        #endregion

        #endregion

        #region Constructors
        static MediaPlayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MediaPlayer), new FrameworkPropertyMetadata(typeof(MediaPlayer)));
        }

        public MediaPlayer()
        {
            IsVisibleChanged += (s, e) =>
            {
                //if (IsVisible) Play();
                //else 
                if (!IsVisible) Stop();
            };

            MediaEndedHandler = (s, e) =>
            {
                if (s is MediaElement)
                    ((MediaElement)s).Position = TimeSpan.FromTicks(1);
            };

            ContextChanged = (s, e) => DataContext = e;

            JumpCommand = new DelegateCommand<object>((obj) =>
            {
                if (obj is TimeSpan)
                {
                    Player.Position = ((TimeSpan)obj);
                    InternalPosition = Player.Position;
                }
            });
        }
        #endregion

        #region Methods

        #region Overrides
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            try
            {
                Player = GetTemplateChild("PART_Player") as MediaElement;
                Player.Position = TimeSpan.FromTicks(1);
                Player.SpeedRatio = SpeedRatio;
                Player.LoadedBehavior = MediaState.Manual;
                Player.UnloadedBehavior = MediaState.Manual;
                

                if (Loop) Player.MediaEnded += MediaEndedHandler;
                Player.MediaOpened += (s, e) => MediaDuration = Player.NaturalDuration.TimeSpan.TotalSeconds;

                #region Play Button
                PlayButton = GetTemplateChild("PART_PlayButton") as Button;
                PlayButton.Click += (s, e) =>
                {
                    if (!Active) return;
                    if (IsPlaying) Pause();
                    else
                    {
                        Player.Play();
                        IsPlaying = true;
                    }
                };
                #endregion

                #region Jump Buttons
                JumpBackward = GetTemplateChild("PART_JumpBackward") as Button;
                JumpBackward.Click += (s, e) =>
                {
                    if (!Active) return;
                    TimeSpan newValue = Player.Position.Subtract(TimeSpan.FromSeconds(JUMP_LENGTH));

                    if (newValue.TotalSeconds < 0) newValue = TimeSpan.FromSeconds(0);

                    Player.Position = newValue;
                    InternalPosition = Player.Position;
                };

                JumpForward = GetTemplateChild("PART_JumpForward") as Button;
                JumpForward.Click += (s, e) =>
                {
                    if (!Active) return;
                    TimeSpan newValue = Player.Position.Add(TimeSpan.FromSeconds(JUMP_LENGTH));

                    if (newValue.TotalSeconds > MediaDuration) newValue = TimeSpan.FromSeconds(MediaDuration);

                    Player.Position = newValue;
                    InternalPosition = Player.Position;
                };
                #endregion

                #region Slider
                SliderPanel = GetTemplateChild("PART_SliderContainer") as Panel;
                Slider = GetTemplateChild("PART_Slider") as Slider;

                bool NewSliderValue = false;

                SliderPanel.PreviewMouseDown += (s, e) =>
                {
                    if (Slider.IsMouseCaptureWithin) return;
                    NewSliderValue = true;
                };

                Slider.ValueChanged += (s, e) =>
                {
                    if (NewSliderValue)
                    {
                        Player.Position = TimeSpan.FromSeconds(Slider.Value);
                        Core.Log.Debug($"New value {Slider.Value}");
                    }
                    NewSliderValue = false;

                };

                Slider.IsMouseCaptureWithinChanged += (s, e) =>
                {
                    if (!(e.NewValue is bool)) return;

                    DragActive = ((bool)e.NewValue);
                    if (!DragActive) Player.Position = TimeSpan.FromSeconds(Slider.Value);
                    else
                    {
                        HoverPosition = TimeSpan.FromSeconds(Slider.Value);
                        HoverPoint = (Slider.Value * Slider.ActualWidth) / Slider.Maximum;
                    }
                };

                SliderPanel.MouseMove += (s, e) =>
                {
                    var point = Mouse.GetPosition(Slider);
                    double width = Slider.ActualWidth;

                    double position = (point.X / width) * Slider.Maximum;
                    HoverPosition = TimeSpan.FromSeconds(position);
                    HoverPoint = point.X;

                    if (Media != null && Media.Context != null)
                    {
                        
                        TimeSpan dif = TimeSpan.FromSeconds(1);
                        HoverPlacemark = Media.Context.Placemarks.FirstOrDefault(p => p.MouseHit);
                    }
                    else HoverPlacemark = null;
                };
                #endregion

                #region Speed Slider
                SpeedSliderPanel = GetTemplateChild("PART_SpeedSliderContainer") as Panel;
                SpeedSlider = GetTemplateChild("PART_SpeedSlider") as Slider;

                bool NewSpeedSliderValue = false;
                SpeedSliderPanel.PreviewMouseDown += (s, e) =>
                {
                    if (SpeedSliderPanel.IsMouseCaptureWithin) return;
                    NewSpeedSliderValue = true;
                };

                SpeedSlider.ValueChanged += (s, e) =>
                {
                    if (NewSpeedSliderValue) SpeedRatio = SpeedSlider.Value;
                    if (NewSpeedSliderValue) Core.Log.Debug($"New SpeedSlider value {SpeedSlider.Value}");
                    NewSpeedSliderValue = false;
                };

                SpeedSlider.IsMouseCaptureWithinChanged += (s, e) =>
                {
                    if (!(e.NewValue is bool)) return;

                    SpeedDragActive = ((bool)e.NewValue);
                    if (!SpeedDragActive) SpeedRatio = SpeedSlider.Value;
                    else
                    {
                        SpeedHoverPosition = SpeedSlider.Value;
                        SpeedHoverPosition = (SpeedSlider.Value * SpeedSlider.ActualWidth) / SpeedSlider.Maximum;
                    }
                };

                SpeedSliderPanel.MouseMove += (s, e) =>
                {
                    var point = Mouse.GetPosition(SpeedSlider);
                    double position = (point.X / SpeedSlider.ActualWidth) * SpeedSlider.Maximum;

                    SpeedHoverPosition = position;
                    SpeedHoverPoint = point.X;
                };
                #endregion

                #region Title Panel
                TitlePanel = GetTemplateChild("PART_TitlePanel") as Panel;
                TitlePanel.MouseDown += (s, e) =>
                {
                    try { Application.Current.MainWindow.DragMove(); }
                    catch (Exception ex) { Core.Log.Error(ex); }
                };
                #endregion

                #region Idea Button
                IdeaButton = GetTemplateChild("PART_IdeaButton") as Button;
                IdeaButton.Click += (s, e) =>
                {
                    RippleControl control = Player.FindParent<RippleControl>();
                    if (control == null) return;

                    control.Play(this);
                };
                #endregion

                #region Speed Button
                SpeedButton = GetTemplateChild("PART_SpeedButton") as Button;
                SpeedButton.Click += (s, e) =>
                {
                    if (Media == null) return;
                    SpeedRatio = Media.Context.Speed;
                };
                #endregion

                #region Back Button
                BackButton = GetTemplateChild("PART_BackButton") as Button;
                BackButton.Click += (s, e) => BackInvoked?.Invoke(this, EventArgs.Empty);
                #endregion
            }
            catch (Exception ex) { Core.Log.Debug($"Failed to apply template in a media player \n{ex}"); return; }
            Active = true;
            Initialize();
        }
        #endregion

        #region Controls
        public async void Initialize()
        {
            if (!Active) return;
            if (Media == null) return;
            if (!await Media.Construct()) return;
            Player.Source = Media.LocalSource;
            SpeedRatio = Media.Context.Speed;
            Player.Play();
            Player.Pause();
            InitializeTimer();
        }

        public void Play()
        {
            if (!Active) return;
            if (Media == null) return;
            Initialize();
            Player.Play();
            IsPlaying = true;
        }

        public void Pause()
        {
            if (!Active) return;
            if (Media == null) return;
            Player.Pause();
            IsPlaying = false;
        }

        public async void Stop(bool decontruct = true)
        {
            if (!Active) return;
            if (Media == null) return;

            try
            {
                Player.Stop();
            }
            catch (Exception ex)
            {
                Core.Log.Error($"Failed to stop a media. \n{ex}\n{Player.LoadedBehavior}");
            }
            
            Player.ClearValue(MediaElement.SourceProperty);
            IsPlaying = false;
            if (decontruct) await Media.Deconstruct();
        }
        #endregion

        void InitializeTimer()
        {
            if (PlayTimer == null)
                PlayTimer = new Timer((s) =>
                {
                    if (DragActive) return;
                    Context.Send((se) => 
                    {
                        if (!IsPlaying) return;
                        InternalPosition = Player.Position;
                        CurrentPlacemark = Media.Context.Placemarks.LastOrDefault(p => p.Start < InternalPosition);
                    }, null);
                }, null, TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(PLAY_INTERVAL));
        }

        void RaisePropertyChanged([CallerMemberName]string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
