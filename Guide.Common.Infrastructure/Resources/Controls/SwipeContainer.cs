using Guide.Common.Infrastructure.Resources.AttachedProperties;
using Guide.Common.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    public enum Direction { Left, Up, Right, Down }

    public class SwipeContainer : Control
    {
        #region Properties

        #region Dependency Properties
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(SwipeContainer), new FrameworkPropertyMetadata(null, (s, e) =>
            {
                Core.Log.Debug("Content Changed");
                if (!(s is SwipeContainer)) return;

                ((SwipeContainer)s).OnContentChanged(e.OldValue, e.NewValue);
            }));


        public Direction SwipeDirection
        {
            get { return (Direction)GetValue(SwipeDirectionProperty); }
            set { SetValue(SwipeDirectionProperty, value); }
        }
        public static readonly DependencyProperty SwipeDirectionProperty =
            DependencyProperty.Register("SwipeDirection", typeof(Direction), typeof(SwipeContainer), new UIPropertyMetadata(Direction.Right, (s, e) =>
            {
                if (s is SwipeContainer) ((SwipeContainer)s).ResetSwipeContainer();
            }));
        #endregion

        #region Internals

        bool IsActive { get; set; }

        #region Elements
        FrameworkElement TransitionDummy { get; set; }
        ContentPresenter ContentControl { get; set; }
        ContentPresenter SwipeControl { get; set; }
        #endregion

        #endregion

        #endregion

        #region Constructors
        static SwipeContainer() => DefaultStyleKeyProperty.OverrideMetadata(typeof(SwipeContainer), new FrameworkPropertyMetadata(typeof(SwipeContainer)));

        public SwipeContainer() { }
        #endregion

        #region Methods

        #region Overrides
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            try
            {
                TransitionDummy = GetTemplateChild("PART_TransitionDummy") as FrameworkElement;
                ContentControl = GetTemplateChild("PART_Container1") as ContentPresenter;
                SwipeControl = GetTemplateChild("PART_Container2") as ContentPresenter;
            }
            catch (Exception ex)
            {
                Core.Log.Debug(ex);
            }
            IsActive = true;
            ResetSwipeContainer();

            if (Content != null)
                OnContentChanged(null, Content);

        }

        async void OnContentChanged(object oldContent, object newContent)
        {
            if (!IsActive) return;

            if (oldContent == newContent) return;

            // Don't swipe the first time
            if (Content == null)
            {
                ContentControl.Content = Content;
                return;
            }

            if (oldContent is IView && !((IView)oldContent).IsAnimatable)
                ContentControl.Content = null;//new Image { Source = ScreenshotElement(this) };

            if (newContent is IView)
            {
                if (((IView)Content).IsAnimatable) SwipeControl.Content = newContent;
                else SwipeControl.Content = null;
            }
            else SwipeControl.Content = newContent;

            if (oldContent is IView) ((IView)oldContent).Close();

            await Swipe();
            SwapContainers();
            ResetSwipeContainer();

            
            
            if (Content is IView)
            {
                if (!((IView)Content).IsAnimatable) ContentControl.Content = newContent;
                await Task.Delay(100);
                Core.Log.Debug("Opening View...");
                ((IView)Content).Open();
                Core.Log.Debug(Content);
            }
        }

        async Task Swipe()
        {
            ExponentialEase ease = new ExponentialEase() { EasingMode = EasingMode.EaseInOut };
            DoubleAnimation x = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(.5))) { EasingFunction = ease };
            DoubleAnimation y = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(.5))) { EasingFunction = ease };


            //x.EasingFunction = new EasingDoubleKeyFrame(,)
            bool completed = false;
            Storyboard storyboard = new Storyboard() { FillBehavior = FillBehavior.Stop };
            storyboard.Children.Add(x);
            storyboard.Children.Add(y);

            Storyboard.SetTarget(x, SwipeControl);
            Storyboard.SetTarget(y, SwipeControl);

            Storyboard.SetTargetProperty(x, new PropertyPath(Directions.HorizontalDirectionProperty));
            Storyboard.SetTargetProperty(y, new PropertyPath(Directions.VerticalDirectionProperty));

            storyboard.Completed += (s, e) => completed = true;
            await Application.Current.Dispatcher.InvokeAsync(() => storyboard.Begin());

            while (!completed) await Task.Delay(10);
        }

        void ResetSwipeContainer()
        {
            if (!IsActive) return;

            double x = .0;
            double y = .0;

            switch (SwipeDirection)
            {
                case Direction.Left: x = -1; break;
                case Direction.Right: x = 1; break;
                case Direction.Up: y = -1; break;
                case Direction.Down: y = 1; break;
            }

            Directions.SetHorizontalDirection(SwipeControl, x);
            Directions.SetVerticalDirection(SwipeControl, y);

            Directions.SetHorizontalDirection(ContentControl, 0);
            Directions.SetVerticalDirection(ContentControl, 0);

            Core.Log.Debug($"Swipe Control {Directions.GetHorizontalDirection(SwipeControl)}, {Directions.GetVerticalDirection(SwipeControl)}");
            Core.Log.Debug($"Content Control {Directions.GetHorizontalDirection(ContentControl)}, {Directions.GetVerticalDirection(ContentControl)}");

            Panel.SetZIndex(SwipeControl, 2);
            Panel.SetZIndex(ContentControl, 1);
        }
        #endregion

        void SwapContainers()
        {
            var temp = SwipeControl;
            SwipeControl = ContentControl;
            ContentControl = temp;
        }

        ImageSource ScreenshotElement(FrameworkElement element)
        {
            RenderTargetBitmap renderer = new RenderTargetBitmap((int)element.ActualWidth, (int)element.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderer.Render(element);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderer));
            BitmapImage image = new BitmapImage();

            using (var fileStream = new FileStream("mb_image.png", FileMode.Create, FileAccess.Write))
                encoder.Save(fileStream);
            /*
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);

                stream.Seek(0, SeekOrigin.Begin);

                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
            }*/
            return image;
        }

        #endregion
    }
}
