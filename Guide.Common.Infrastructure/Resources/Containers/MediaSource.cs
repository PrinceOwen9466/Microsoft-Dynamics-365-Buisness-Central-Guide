using Guide.Common.Infrastructure.Resources.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Local = Guide.Common.Infrastructure.Resources.Controls;

namespace Guide.Common.Infrastructure.Resources.Containers
{
    [ContentProperty("Context")]
    public class MediaSource : DependencyObject, IUriContext
    {
        #region Properties

        #region Events
        public event EventHandler SourceChanged;
        public event EventHandler<MediaContext> ContextChanged;
        public event EventHandler ThumbnailLoaded;
        #endregion

        #region IUriContext Implementation
        Uri _baseUri = null;
        public Uri BaseUri
        {
            get => _baseUri;
            set
            {
                _baseUri = value;
                if (value != null && UriSource != null) UriSource = UriSource;
            }
        }
        #endregion

        #region Dependency Properties
        public Uri UriSource
        {
            get { return (Uri)GetValue(UriSourceProperty); }
            set
            {
                /*
                try
                {
                    //Uri folder = new Uri(BaseUri, ".");
                    value = new Uri(folder, value.OriginalString);
                }
                catch (Exception ex) { Core.Log.Error($"An error occured while concatinating two uris.\n{ex}"); }
                */
                SetValue(UriSourceProperty, value);
            }
        }

        public static readonly DependencyProperty UriSourceProperty =
            DependencyProperty.Register("UriSource", typeof(Uri), typeof(MediaSource), new UIPropertyMetadata(null, (s, e) =>
            {
                if (!(s is MediaSource)) return;
                MediaSource mediaSource = (MediaSource)s;
                if (mediaSource.BaseUri == null) return;

                var uriSource = (Uri)e.NewValue;

                Uri folder = new Uri(mediaSource.BaseUri, ".");
                uriSource = new Uri(folder, uriSource.OriginalString);
                if (uriSource == mediaSource.UriSource) return;
                mediaSource.UriSource = uriSource;
                mediaSource.SourceChanged?.Invoke(mediaSource, EventArgs.Empty);
            }));

        public Uri LocalSource
        {
            get { return (Uri)GetValue(LocalSourceProperty); }
            private set { SetValue(LocalSourceProperty, value); }
        }

        public static readonly DependencyProperty LocalSourceProperty =
            DependencyProperty.Register("LocalSource", typeof(Uri), typeof(MediaSource), new UIPropertyMetadata(null));
        

        public MediaContext Context
        {
            get { return (MediaContext)GetValue(ContextProperty); }
            set { SetValue(ContextProperty, value); }
        }

        public static readonly DependencyProperty ContextProperty =
            DependencyProperty.Register("Context", typeof(MediaContext), typeof(MediaSource), new UIPropertyMetadata(null, (s, e) =>
            {
                if (s is MediaSource) ((MediaSource)s).ContextChanged?.Invoke(s, (MediaContext)e.NewValue);
            }));




        public BitmapSource ThumbnailSource
        {
            get { return (BitmapSource)GetValue(ThumbnailSourceProperty); }
            set { SetValue(ThumbnailSourceProperty, value); }
        }

        public static readonly DependencyProperty ThumbnailSourceProperty =
            DependencyProperty.Register("ThumbnailSource", typeof(BitmapSource), typeof(MediaSource), new UIPropertyMetadata(null));


        #endregion

        #region Internals
        bool IsActive { get; set; }
        FileInfo LocalSourceInfo { get; set; }
        #endregion

        #region Statics
        static readonly Size ThumbnailSize = new Size(32, 32);
        #endregion

        #endregion

        #region Methods

        #region Controls
        public async Task<bool> Construct()
        {
            if (UriSource == null) return false;
            if (IsActive) return false;

            try
            {
                string source = UriSource.OriginalString;
                string extension = Path.GetExtension(source);
                string outputPath = Path.Combine(Core.TEMP_DIR, Guid.NewGuid().ToString()) + extension;

                #region Embedded Resource Extraction                
                /*
                string assemblyPath = Path.Combine(Core.BASE_DIR, UriSource.Segments[1].Split(';')[0]) + ".dll";

                if (!File.Exists(assemblyPath)) return false;

                try
                {

                    System.Windows.Resources.StreamResourceInfo resourceInfo = null;
                    if (UriSource.ToString().IndexOf("siteoforigin", StringComparison.OrdinalIgnoreCase) >= 0)
                        resourceInfo = Application.GetRemoteStream(UriSource);
                    else resourceInfo = Application.GetResourceStream(UriSource);


                }
                catch (Exception ex) { Core.Log.Debug(ex); }



                Assembly assembly = Assembly.LoadFrom(assemblyPath);
                //var b = Application.GetResourceStream(new Uri("/Icons/Microsoft Buisness Central Guide Icon.ico", UriKind.Relative));
                string path = string.Join("", UriSource.Segments, 1, UriSource.Segments.Length - 1).Replace(";component", "").Replace("%20", " ").Replace('/', '.');

                foreach (var resource in assembly.GetManifestResourceNames())
                    Core.Log.Debug(resource);

                Core.Log.Debug(path);
                //using (Stream resourceStream = assembly.GetManifestResourceStream(path))
                */
                #endregion

                await Task.Run(() =>
                {
                    int read = -1; byte[] buffer = new byte[4096];
                    using (Stream resourceStream = Application.GetResourceStream(new Uri(source)).Stream)
                    using (FileStream outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                        while ((read = resourceStream.Read(buffer, 0, buffer.Length)) > 0)
                            outputStream.Write(buffer, 0, read);
                });
                

                File.SetAttributes(outputPath, FileAttributes.Hidden);
                LocalSource = new Uri(outputPath);
                LocalSourceInfo = new FileInfo(outputPath);
                return true;
            }
            catch (Exception ex)
            {
                Core.Log.Error($"An error occured while constructing a media source.\n{ex}");
                return false;
            }
        }

        public async Task<bool> Deconstruct()
        {
            IsActive = false;
            if (LocalSourceInfo == null || !LocalSourceInfo.Exists) return true;
            string directory = LocalSourceInfo.DirectoryName;

            try
            {
                await Task.Run(() =>
                {
                    using (var watcher = new FileSystemWatcher(directory))
                    using (var resetEvent = new ManualResetEventSlim())
                    {
                        watcher.EnableRaisingEvents = true;
                        watcher.IncludeSubdirectories = true;
                        watcher.Deleted += (s, e) => resetEvent.Set();
                        LocalSourceInfo.Delete();
                        LocalSourceInfo.Refresh();
                        resetEvent.Wait(30000);
                    }
                });
                return true;
            }
            catch (Exception ex)
            {
                Core.Log.Error($"An error occured while attempting to deconstruct a media source.\n{ex}");
                return false;
            }
        }

        public async Task LoadThumbnail()
        {
            bool wasActive = IsActive;

            if (!IsActive && !await Construct()) return;

            Grid panel = new Grid();
            MediaElement element = new MediaElement()
            {
                ScrubbingEnabled = true,
                Source = LocalSource,
                LoadedBehavior = MediaState.Manual,
                UnloadedBehavior = MediaState.Manual
            };

            element.Play();
            element.Pause();

            // Local.MediaPlayer player = new Local.MediaPlayer();

            panel.Children.Add(element);

            /*
            if (!player.IsLoaded)
                player.Loaded += (s, e) =>
                {
                    RenderTargetBitmap renderer = new RenderTargetBitmap((int)ThumbnailSize.Width, (int)ThumbnailSize.Height, 96, 96, PixelFormats.Pbgra32);
                    renderer.Render(player);
                    ThumbnailSource = BitmapFrame.Create(renderer);
                    ThumbnailLoaded?.Invoke(this, EventArgs.Empty);
                };
            else
            {
                RenderTargetBitmap renderer = new RenderTargetBitmap((int)ThumbnailSize.Width, (int)ThumbnailSize.Height, 96, 96, PixelFormats.Pbgra32);
                renderer.Render(player);
                ThumbnailSource = BitmapFrame.Create(renderer);
                ThumbnailLoaded?.Invoke(this, EventArgs.Empty);
            }
            */

            panel.Measure(ThumbnailSize);
            panel.Arrange(new Rect(ThumbnailSize));
            panel.UpdateLayout();


            RenderTargetBitmap renderer = new RenderTargetBitmap((int)ThumbnailSize.Width, (int)ThumbnailSize.Height, 96, 96, PixelFormats.Pbgra32);
            renderer.Render(element);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderer));
            var image = new BitmapImage();

            string output = Path.Combine(Core.TEMP_DIR, Guid.NewGuid() + ".png");
            using (FileStream stream = new FileStream(output, FileMode.CreateNew, FileAccess.Write))
                encoder.Save(stream);

            using (FileStream stream = new FileStream(output, FileMode.Open, FileAccess.Read))
            {
                stream.Position = 0;

                image.BeginInit();
                image.StreamSource = stream;
                image.EndInit();
                image.Freeze();
            }

            ThumbnailSource = image;
            ThumbnailLoaded?.Invoke(this, EventArgs.Empty);




            if (!wasActive) await Deconstruct();
        }
        #endregion

        #endregion
    }
}
