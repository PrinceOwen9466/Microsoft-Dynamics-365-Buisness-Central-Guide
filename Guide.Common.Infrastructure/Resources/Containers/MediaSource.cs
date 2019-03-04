using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Guide.Common.Infrastructure.Resources.Containers
{
    public class MediaSource : DependencyObject, IUriContext
    {
        #region Properties

        #region Events
        public event EventHandler SourceChanged;
        public event EventHandler<MediaContext> ContextChanged;
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


        #endregion

        #region Internals
        bool IsActive { get; set; }
        FileInfo LocalSourceInfo { get; set; }
        #endregion

        #endregion

        #region Methods

        #region Controls
        public bool Construct()
        {
            if (UriSource == null) return false;
            if (IsActive) return false;

            try
            {
                string extension = Path.GetExtension(UriSource.OriginalString);
                string outputPath = Path.Combine(Core.TEMP_DIR, Guid.NewGuid().ToString()) + extension;
                int read = -1; byte[] buffer = new byte[4096];

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

                using (Stream resourceStream = Application.GetResourceStream(UriSource).Stream)
                using (FileStream outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                    while ((read = resourceStream.Read(buffer, 0, buffer.Length)) > 0)
                        outputStream.Write(buffer, 0, read);

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

        public bool Deconstruct()
        {
            IsActive = false;
            if (LocalSourceInfo == null || !LocalSourceInfo.Exists) return true;
            try
            {
                using (var watcher = new FileSystemWatcher(LocalSourceInfo.DirectoryName))
                using (var resetEvent = new ManualResetEventSlim())
                {
                    watcher.EnableRaisingEvents = true;
                    watcher.IncludeSubdirectories = true;
                    watcher.Deleted += (s, e) => resetEvent.Set();
                    LocalSourceInfo.Delete();
                    LocalSourceInfo.Refresh();
                    resetEvent.Wait(30000);
                }
                return true;
            }
            catch (Exception ex)
            {
                Core.Log.Error($"An error occured while attempting to deconstruct a media source.\n{ex}");
                return false;
            }
        }
        #endregion

        #endregion
    }
}
