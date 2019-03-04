using CommonServiceLocator;
using Guide.Common.Infrastructure;
using Guide.Common.Infrastructure.Resources.Interfaces;
using Guide.Common.Infrastructure.Services;
using Guide.Common.Infrastructure.Services.Interfaces;
using Guide.ContentLibrary;
using Guide.Desktop.Views;
using Prism.Ioc;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace Guide.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        #region Properties
        
        #endregion

        #region Constructors
        public App()
        {

            /*
            #region CefSharp Initialization
            //Add Custom assembly resolver
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;

            //Any CefSharp references have to be in another method with NonInlining
            // attribute so the assembly rolver has time to do it's thing.
            InitializeCefSharp();
            #endregion
            */
            
        }
        #endregion

        #region Methods

        /*
        #region CefSharp
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void InitializeCefSharp()
        {
            var settings = new CefSettings();

            // Set BrowserSubProcessPath based on app bitness at runtime
            settings.BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                   Environment.Is64BitProcess ? "x64" : "x86",
                                                   "CefSharp.BrowserSubprocess.exe");
            
            //Uri uri = new Uri(@"/Guide.ContentLibrary;component/Resources/Pages/index.html");

           // ContentLibrary.ContentCore.RegisterSettings(ref settings);
            // Make sure you set performDependencyCheck false
            Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);
        }

        // Will attempt to load missing assembly from either x86 or x64 subdir
        // Required by CefSharp to load the unmanaged dependencies when running using AnyCPU
        private static Assembly Resolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                       Environment.Is64BitProcess ? "x64" : "x86",
                                                       assemblyName);

                return File.Exists(archSpecificPath)
                           ? Assembly.LoadFile(archSpecificPath)
                           : null;
            }

            return null;
        }
        #endregion
        */

        #region Overrides

        public void InitializePrism()
        {
            Core.Log.Debug("Initializing Prism...");
            ConfigureViewModelLocator();
            Initialize();

            ContentCore.Initialize();
            ServiceLocator.Current.GetInstance<IPresenter>().Initialize(ContentCore.Container, Container.Resolve<HomeView>());
        }

        public void RaiseOnInitialized()
        {
            OnInitialized();
            Core.Log.Debug("Prism has been successfully initialized...");
            

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //base.OnStartup(e);
            Core.Initialize();
            

            Core.Log.Info("Initializing Gecko Engine...");
            try { Gecko.Xpcom.Initialize("Firefox"); }
            catch (Exception ex) { Core.Log.Error($"An error occured while attempting to initialize the Gecko Engine\n{ex}"); throw; }
            Core.Log.Info("Initialization Complete...");

            this.Dispatcher.UnhandledException += (s, ex) => Core.Log.Error($"An unhandled exception occured in the dispatcher\n{ex.Exception}");
            AppDomain.CurrentDomain.UnhandledException += (s, ex) => Core.Log.Error("An unhandled exception occured in the app domain\n{0}", ex.ExceptionObject);
            this.DispatcherUnhandledException += (s, ex) => Core.Log.Error(ex.Exception, "An unhandled exception occured in the application\n{0}", ex.Exception);
            TaskScheduler.UnobservedTaskException += (s, ex) => Core.Log.Error(ex.Exception, "An unhandled exception occured in the task scheduler\n{0}", ex.Exception);
        }

        protected override Window CreateShell() => (Views.Shell)MainWindow;//ServiceLocator.Current.TryResolve<Views.Shell>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ILoggerFacade, NLogger>();
            
            containerRegistry.RegisterSingleton<IPresenter, Presenter>();
            containerRegistry.RegisterSingleton<IConfigurationManager, XMLConfigurationManager>();
            containerRegistry.RegisterSingleton<ISearchEngine, SearchEngine>();
        }
        #endregion

        #endregion
    }
}
