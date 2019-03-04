using Guide.Common.Infrastructure.Extensions;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    public class GifImage : Image
    {
        #region Properties

        #region Dependency Properties



        public string GifResource
        {
            get { return (string)GetValue(GifResourceProperty); }
            set { SetValue(GifResourceProperty, value); }
        }

        public static readonly DependencyProperty GifResourceProperty =
            DependencyProperty.Register("GifResource", typeof(string), typeof(GifImage), new UIPropertyMetadata(string.Empty));



        /*
        public BitmapImage GifSource
        {
            get { return (BitmapImage)GetValue(GifSourceProperty); }
            set { SetValue(GifSourceProperty, value); }
        }

        public static readonly DependencyProperty GifSourceProperty =
            DependencyProperty.Register("GifSource", typeof(BitmapImage), typeof(GifImage), new UIPropertyMetadata(null));
            */


        public Assembly AssemblySource
        {
            get { return (Assembly)GetValue(AssemblySourceProperty); }
            set { SetValue(AssemblySourceProperty, value); }
        }
        public static readonly DependencyProperty AssemblySourceProperty =
            DependencyProperty.Register("AssemblySource", typeof(Assembly), typeof(GifImage), new UIPropertyMetadata(Assembly.GetExecutingAssembly()));




        public int FramesPerSecond
        {
            get { return (int)GetValue(FramesPerSecondProperty); }
            set { SetValue(FramesPerSecondProperty, value); }
        }

        public static readonly DependencyProperty FramesPerSecondProperty =
            DependencyProperty.Register("FramesPerSecond", typeof(int), typeof(GifImage), new UIPropertyMetadata(30));




        public int FrameIndex
        {
            get { return (int)GetValue(FrameIndexProperty); }
            set { SetValue(FrameIndexProperty, value); }
        }
        public static readonly DependencyProperty FrameIndexProperty =
            DependencyProperty.Register("FrameIndex", typeof(int), typeof(GifImage), new UIPropertyMetadata(0, (s, e) =>
            {
                var image = s as GifImage;
                image.Source = image.Decoder.Frames[(int)e.NewValue];
            }));

        //propdp

        #endregion

        #region Internals
        bool Ready { get; set; }
        GifBitmapDecoder Decoder { get; set; }
        Int32Animation Animation { get; set; }
        #endregion

        #endregion

        #region Constructors
        static GifImage()
        {
            VisibilityProperty.OverrideMetadata(typeof(GifImage), new FrameworkPropertyMetadata((s, e) =>
            {
                if (!(s is GifImage)) return;
                Core.Log.Debug("Override Visibility called");
                /*
                if ((Visibility)e.NewValue == Visibility.Visible)
                    ((GifImage)s).StartAnimation();
                else ((GifImage)s).StopAnimation();
                */
            }));
        }

        public GifImage()
        {
            Loaded += (s, e) => Initialize();
            IsVisibleChanged += (s, e) =>
            {
                Core.Log.Debug("Visibility called");
                if (Visibility == Visibility.Visible)
                    ((GifImage)s).StartAnimation();
                else ((GifImage)s).StopAnimation();
            };
        }
        #endregion

        #region Methods
        void Initialize()
        {
            if (Ready) return;
            Core.Log.Debug("Initializing...");

            var stream = ResourceHelper.GetEmbeddedResourceStream(GifResource, AssemblySource);

            //string packUri = "pack://application:,,,Guide.ContentLibrary;component/Resources/";
            //Uri uri = new Uri(packUri + GifSource.UriSource, UriKind.Absolute);
            
            Decoder = new GifBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            int frameCount = Decoder.Frames.Count;
            Animation = new Int32Animation(0, frameCount - 1, new Duration(new TimeSpan(0, 0, 0, frameCount / FramesPerSecond, (int)((frameCount / (double)FramesPerSecond - frameCount / FramesPerSecond) * 1000))));
            Animation.RepeatBehavior = RepeatBehavior.Forever;
            Source = Decoder.Frames[0];

            Ready = true;
        }

        void StartAnimation()
        {
            if (!Ready) Initialize();
            BeginAnimation(FrameIndexProperty, Animation);
        }

        void StopAnimation()
        {
            BeginAnimation(FrameIndexProperty, null);
        }
        #endregion
    }
}
