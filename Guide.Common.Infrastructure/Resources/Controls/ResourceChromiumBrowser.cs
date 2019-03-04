//using CefSharp;
//using CefSharp.Wpf;
using Guide.Common.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    /*
    public class ResourceChromiumBrowser : ChromiumWebBrowser
    {
        #region Properties

        #region Dependency Properties
        public ResourceContainer ResourceContainer
        {
            get { return (ResourceContainer)GetValue(ResourceContainerProperty); }
            set { SetValue(ResourceContainerProperty, value); }
        }

        public static readonly DependencyProperty ResourceContainerProperty =
            DependencyProperty.Register("ResourceContainer", typeof(ResourceContainer), typeof(ResourceChromiumBrowser), new UIPropertyMetadata(null));




        public string DefaultPage
        {
            get { return (string)GetValue(DefaultPageProperty); }
            set { SetValue(DefaultPageProperty, value); }
        }

        public static readonly DependencyProperty DefaultPageProperty =
            DependencyProperty.Register("DefaultPage", typeof(string), typeof(ResourceChromiumBrowser), new PropertyMetadata(null));



        public bool LoadComplete
        {
            get { return (bool)GetValue(LoadCompleteProperty); }
            set { SetValue(LoadCompleteProperty, value); }
        }
        public static readonly DependencyProperty LoadCompleteProperty =
            DependencyProperty.Register("LoadComplete", typeof(bool), typeof(ResourceChromiumBrowser), new UIPropertyMetadata(false));


        #endregion

        #endregion

        #region Constructors
        public ResourceChromiumBrowser()
        {


            RoutedEventHandler loaded = null;
            Loaded += loaded = (s, e) =>
            {
                Loaded -= loaded;
                
               
            };


            LoadingStateChanged += (s, e) =>
            {
                Application.Current.Dispatcher.Invoke(() => LoadComplete = !e.IsLoading);
                
            };
            



            DependencyPropertyChangedEventHandler initialized = null;
            IsBrowserInitializedChanged += initialized = (s, e) =>
            {
                IsBrowserInitializedChanged -= initialized;
                if (ResourceContainer == null) return;
                ResolveResources();

                if (!string.IsNullOrWhiteSpace(DefaultPage))
                    Load(DefaultPage);


                
            };
        }
        #endregion

        #region Methods

        void ResolveResources()
        {
            for (int i = 0; i < ResourceContainer.Resources.Count; i++)
            {

                try
                {
                    var resource = ResourceContainer.Resources[i];
                    string path = ResourceContainer.ResourceFolder + '/' + resource.Source;
                    this.RegisterResourceHandler(ResourceContainer.CustomScheme + '/' + resource.Source, 
                        ResourceHelper.GetEmbeddedResourceStream(path, ResourceContainer.ResourceAssembly), 
                        resource.MimeType, resource.OneTimeUse);
                }
                catch { } 
            }
        }


        #endregion
    }

    public class ResourceContainer
    {
        #region Properties
        public string CustomScheme { get; set; }
        public string ResourceFolder { get; set; }
        public Assembly ResourceAssembly { get; set; }
        public List<WebResource> Resources { get; set; } = new List<WebResource>();
        #endregion

        #region Constructors
        public ResourceContainer() { }
        #endregion 
    }

    public class WebResource
    {
        public string Source { get; set; }
        public string MimeType { get; set; } = "text/html";
        public bool OneTimeUse { get; set; }
    }
    */
}
