//using CefSharp;
//using CefSharp.SchemeHandler;
//using CefSharp.Wpf;
using Guide.Common.Infrastructure;
using Guide.Common.Infrastructure.Extensions;
using Guide.Common.Infrastructure.Services.Interfaces;
using Guide.ContentLibrary.Views.Business_Central_Overview;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Guide.ContentLibrary.ViewModels.Business_Central_Overview
{
    public class HomePageViewModel : BindableBase, IViewAware
    {
        #region Properties

        #region Bindables

        IView _view = null;
        public IView View
        {
            get => _view;
            set
            {
                if (value == null) return;
                _view = value;
                
                if (_view is HomePage)
                {
                    /*
                    var browser = ((HomePage)_view).Browser;
                    browser.RegisterResourceHandler(@"http://custom/index.html", ResourceHelper.GetEmbeddedResourceStream("Resources/Pages/index.html", ContentCore.ContentAssembly));
                    browser.IsBrowserInitializedChanged += (s, e) =>
                    {
                        if (!browser.IsBrowserInitialized) return;
                        browser.Load(@"http://custom/index.html");
                    };
                    */
                    /*
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "Resources/Pages/index.html");
                    
                    
                    
                    */


                    //string content = "data:text/html," + EscapeUriDataString(PageContent);
                    //Clipboard.SetText(content);
                    //var f = ((HomePage)_view).Browser.BrowserSettings
                    //CefSettings s;s.reg

                    //FolderSchemeHandlerFactory d = new FolderSchemeHandlerFactory("");

                    //ISchemeHandlerFactory
                    
                }
                
            }
        }

        #region Commands
        public ICommand LoadedCommand { get; }
        #endregion
       
        #endregion

        #region Internals
        string PageContent { get; set; }
        #endregion

        #endregion

        #region Constructors
        public HomePageViewModel()
        {
            /*
            PageContent = ResourceHelper.GetEmbeddedResource(@"Resources/Pages/Microsoft Business Central Home Help.html", typeof(Sections).Assembly);

            LoadedCommand = new DelegateCommand<object>(OnLoaded);
            */
        }
        #endregion

        #region Methods

        #region Command Handlers

        #endregion

        string EscapeUriDataString(string data)
        {
            int limit = 65519;
            int loops = data.Length / limit;
            StringBuilder builder = new StringBuilder();
            
            for (int i = 0; i <= loops; i++)
                builder.Append(Uri.EscapeDataString(i < loops ? data.Substring(limit * i, limit)
                    : data.Substring(limit * i)));
            return builder.ToString();
        }

        #endregion
    }
}
