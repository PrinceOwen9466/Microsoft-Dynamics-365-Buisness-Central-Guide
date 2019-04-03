using CommonServiceLocator;
using Guide.Common.Infrastructure.Extensions;
using Guide.Common.Infrastructure.Services.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    public class RippleControl : PresentationControl
    {
        #region Properties

        #region Dependency Properties
        public Brush TransitionBrush
        {
            get { return (Brush)GetValue(TransitionBrushProperty); }
            set { SetValue(TransitionBrushProperty, value); }
        }

        public static readonly DependencyProperty TransitionBrushProperty =
            DependencyProperty.Register("TransitionBrush", typeof(Brush), typeof(RippleControl), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));

        public double StartPointX
        {
            get { return (double)GetValue(StartPointXProperty); }
            set { SetValue(StartPointXProperty, value); }
        }

        public static readonly DependencyProperty StartPointXProperty =
            DependencyProperty.Register("StartPointX", typeof(double), typeof(RippleControl), new UIPropertyMetadata(0.0));


        public double StartPointY
        {
            get { return (double)GetValue(StartPointYProperty); }
            set { SetValue(StartPointYProperty, value); }
        }

        public static readonly DependencyProperty StartPointYProperty =
            DependencyProperty.Register("StartPointY", typeof(double), typeof(RippleControl), new UIPropertyMetadata(0.0));



        public TimeSpan TransitionDuration
        {
            get { return (TimeSpan)GetValue(TransitionDurationProperty); }
            set { SetValue(TransitionDurationProperty, value); }
        }

        public static readonly DependencyProperty TransitionDurationProperty =
            DependencyProperty.Register("TransitionDuration", typeof(TimeSpan), typeof(RippleControl), new UIPropertyMetadata(TimeSpan.FromMilliseconds(1000)));


        public object ExtraContent
        {
            get { return (object)GetValue(ExtraContentProperty); }
            set { SetValue(ExtraContentProperty, value); }
        }

        public static readonly DependencyProperty ExtraContentProperty =
            DependencyProperty.Register("ExtraContent", typeof(object), typeof(RippleControl), new UIPropertyMetadata(null));



        public bool PlayerActive
        {
            get { return (bool)GetValue(PlayerActiveProperty); }
            set { SetValue(PlayerActiveProperty, value); Core.Log.Debug(PlayerActive); }
        }
        public static readonly DependencyProperty PlayerActiveProperty =
            DependencyProperty.Register("PlayerActive", typeof(bool), typeof(RippleControl), new UIPropertyMetadata(false));



        public bool RippleActive
        {
            get { return (bool)GetValue(RippleActiveProperty); }
            private set { SetValue(RippleActiveProperty, value); }
        }
        public static readonly DependencyProperty RippleActiveProperty =
            DependencyProperty.Register("RippleActive", typeof(bool), typeof(RippleControl), new UIPropertyMetadata(false));


        #endregion


        #region Internals

        #region Elements
        Ellipse TransitionEllipse { get; set; }
        Panel ExtraContentContainer { get; set; }
        ContentPresenter ExtraContentPresenter { get; set; }
        Border PlayerBorder { get; set; }

        MediaElement MediaElement { get; set; }
        MediaPlayer Player { get; set; }

        MediaElement OriginalElement { get; set; }

        MediaPlayer OriginalPlayer { get; set; }
        #endregion

        IPresenter Presenter => ServiceLocator.Current.GetInstance<IPresenter>();

        #endregion


        #endregion

        #region Constructors
        static RippleControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RippleControl), new FrameworkPropertyMetadata(typeof(RippleControl)));
        }

        public RippleControl()
        {

        }
        #endregion

        #region Methods

        public void StartTransition()
        {
            TransitToRippleContent();
        }

        #region Animations

        void TransitToRippleContent()
        {
            RippleActive = true;
            ExtraContentContainer.Visibility = Visibility.Visible;
            
            double scaleX = this.ActualWidth / TransitionEllipse.ActualWidth;
            double scaleY = this.ActualHeight / TransitionEllipse.ActualHeight;


            double resultant = Math.Sqrt((Math.Pow(scaleX, 2) + Math.Pow(scaleY, 2)));

            double diameter = resultant * 2;//Math.Max(scaleX, scaleY) * 2;

            DoubleAnimation xAnimation = new DoubleAnimation(diameter, new Duration(TransitionDuration));
            DoubleAnimation yAnimation = new DoubleAnimation(diameter, new Duration(TransitionDuration));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);

            Storyboard.SetTarget(xAnimation, TransitionEllipse);
            Storyboard.SetTarget(yAnimation, TransitionEllipse);
            Storyboard.SetTargetProperty(xAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));

            storyboard.Completed += (s, e) =>
            {
                if (RippleActive) ExtraContentPresenter.Visibility = Visibility.Visible;
            };

            storyboard.Begin();
        }

        void TransitToMainContent()
        {
            RippleActive = false;
            ExtraContentPresenter.Visibility = Visibility.Hidden;

            DoubleAnimation xAnimation = new DoubleAnimation(1, new Duration(TransitionDuration));
            DoubleAnimation yAnimation = new DoubleAnimation(1, new Duration(TransitionDuration));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);

            Storyboard.SetTarget(xAnimation, TransitionEllipse);
            Storyboard.SetTarget(yAnimation, TransitionEllipse);
            Storyboard.SetTargetProperty(xAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));

            storyboard.Completed += (s, e) =>
            {
                ExtraContentContainer.Visibility = Visibility.Hidden;
            };

            storyboard.Begin();
        }

        public override void Close()
        {
            if (!RippleActive)
            {
                base.Close();
                return;
            }

            RippleControl control = this;
            List<RippleControl> controls = new List<RippleControl>();
            while (control != null)
            {
                if (control.PlayerActive) DisablePlayer();
                controls.Add(control);
                if (control.RippleActive && control.ExtraContent is RippleControl)
                    control = (RippleControl)control.ExtraContent;
                else control = null;
            }
            controls.Reverse();

            foreach (var c in controls) c.TransitToMainContent();
            base.Close();
        }


        #endregion

        #region Overrides
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ExtraContentContainer = GetTemplateChild("PART_ExtraContentContainer") as Panel;
            ExtraContentPresenter = GetTemplateChild("PART_ExtraContentPresenter") as ContentPresenter;
            TransitionEllipse = GetTemplateChild("PART_TransitionEllipse") as Ellipse;
            PlayerBorder = GetTemplateChild("PART_PlayerBorder") as Border;


            object player = GetTemplateChild("PART_Player");
            if (player is MediaElement)
                MediaElement = player as MediaElement;
            else if (player is MediaPlayer)
            {
                Player = player as MediaPlayer;
                Player.BackInvoked += (s, e) =>
                {
                    DisablePlayer();
                    if (Presenter != null)
                        Presenter.VideoActive = false;
                };
            }


            ExtraContentContainer.MouseRightButtonDown += (s, e) =>
            {
                if (e.Handled) return;
                e.Handled = true;
                TransitToMainContent();
            };

            /*
            PlayerBorder.PreviewMouseLeftButtonDown += (s, e) =>
            {
                PlayerBorder.Visibility = Visibility.Hidden;

                if (OriginalElement != null)
                {
                    OriginalElement.Position = Player.Position;
                    OriginalElement.Play();
                    OriginalElement.Position = Player.Position;
                }

                Player.Stop();
                Player.ClearValue(MediaElement.SourceProperty);
                OriginalElement = null;
            };
            */

            PlayerBorder.PreviewMouseRightButtonDown += (s, e) =>
            {
                e.Handled = true;
                

                

                if (MediaElement != null)
                {
                    PlayerBorder.Visibility = Visibility.Hidden;
                    MediaElement.Stop();
                    MediaElement.ClearValue(MediaElement.SourceProperty);

                    if (OriginalElement != null)
                    {
                        OriginalElement.Position = MediaElement.Position;
                        OriginalElement.Play();
                        OriginalElement.Position = MediaElement.Position;
                        OriginalElement = null;
                    }
                }

                // if (Player != null) Player.Stop();

                

                
            };
            /*
            var storyboard = GetTemplateChild("SlideInPlayer") as Storyboard;
            storyboard.Completed += (se, ev) =>
            {
                Core.Log.Debug(ev);
            };*/
        }

        public void Play(Uri source, TimeSpan position, double speedRatio = 1)
        {
            PlayerBorder.Visibility = Visibility.Visible;
            MediaElement.Source = source;
            MediaElement.Position = position;
            MediaElement.SpeedRatio = speedRatio;
            MediaElement.Play();
            MediaElement.Position = position;
            OriginalElement = null;
        }

        public void Play(MediaElement originalElement)
        {
            if (originalElement == null) return;

            OriginalElement = originalElement;
            PlayerBorder.Visibility = Visibility.Visible;
            OriginalElement.Pause();
            MediaElement.Source = OriginalElement.Source;
            MediaElement.Position = OriginalElement.Position;
            MediaElement.SpeedRatio = originalElement.SpeedRatio;
            MediaElement.Play();
            MediaElement.Position = OriginalElement.Position;
        }

        public void Play(MediaPlayer originalPlayer)
        {
            if (originalPlayer == null) return;
            if (Player == null) return;

            OriginalPlayer = originalPlayer;
            OriginalPlayer.Pause();

            InitializePlayer();

            if (Presenter != null)
            {
                Presenter.VideoActive = true;
                Presenter.Player = Player;
            }
        }
        #endregion

        void InitializePlayer()
        {
            Player.Media = OriginalPlayer.Media;
            Player.MediaPosition = OriginalPlayer.MediaPosition;
            Player.SpeedRatio = OriginalPlayer.SpeedRatio;

            //PlayerBorder.Visibility = Visibility.Visible;


            /*
            PresentationControl mainControl = this;

            while (true)
            {
                var control = mainControl.FindParent<RippleControl>();
                if (control == null) break;
                if (!control.IsContent) break;
                mainControl = control;
            }

            mainControl.IsFullPage = true;
            */


            if (Presenter != null)
            {
                Presenter.FullPageActive = true;
                // Presenter.RefreshFullView();
            }

            PlayerActive = true;
            Player.Play();
            //await PlayerBorder.AnimateTranslateY(null, 0, TimeSpan.FromSeconds(10));
        }

        void DisablePlayer()
        {
            Player.Stop(false);

            if (OriginalPlayer != null)
            {
                OriginalPlayer.MediaPosition = Player.MediaPosition;
                OriginalPlayer.Play();
                OriginalPlayer = null;
            }
            

            IsFullPage = false;
            

            PlayerActive = false;
            if (Presenter != null)
            {
                Presenter.FullPageActive = false;
                Presenter.Player = null;
                // Presenter.RefreshFullView();
            }
            // PlayerBorder.Visibility = Visibility.Hidden;

            /*
            double transformY = -Application.Current.MainWindow.ActualHeight - 20;

            await PlayerBorder.DoubleAnimation(TranslateTransform.YProperty
                , null, transformY, TimeSpan.FromSeconds(1));
                */


            //((TranslateTransform)Player.RenderTransform).Y = transformY;


        }

        #endregion
    }
}
