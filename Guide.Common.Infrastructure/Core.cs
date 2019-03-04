using Guide.Common.Infrastructure.Services;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guide.Common.Infrastructure
{
    public static class Core
    {
        #region Properties
        public static Logger Log { get; } = LogManager.GetCurrentClassLogger();
        //public static Assembly ContentAssembly => typeof(Sections).Assembly;
        public static bool Initialized { get; private set; }

        #region Solids
        public const string PRODUCT_NAME = "Microsoft Dynamics 365 Buisness Guide";
        public const string AUTHOR = "Prince Owen";

        #region Prism Constants

        #region Regions
        public const string MAIN_REGION = "Main Region";
        public const string CONTENT_REGION = "Content Region";
        #endregion

        #region Views
        public const string HOME_VIEW = "Home View";
        public const string START_VIEW = "Start View";
        public const string MAIN_VIEW = "Main View";
        #endregion

        #endregion

        #region Location Map

        #region Directories
        public readonly static string SYSTEM_DATA_DIR = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public readonly static string BASE_DIR = Directory.GetCurrentDirectory();
        public readonly static string WORK_DIR = Path.Combine(SYSTEM_DATA_DIR, PRODUCT_NAME);
        public readonly static string TEMP_DIR = Path.Combine(WORK_DIR, ".temp");
        public readonly static string DATABASE_DIR = Path.Combine(WORK_DIR, "Data");
        public readonly static string LOG_DIR = Path.Combine(WORK_DIR, "Logs");
        public readonly static string SEARCH_INDEX_DIR = Path.Combine(DATABASE_DIR, "Indexes");
        #endregion

        #region Paths
        public readonly static string CONFIGURATION_PATH = Path.Combine(DATABASE_DIR, "config.xml");
        #endregion

        #region Names
        public const string ERROR_LOG_NAME = "Errors";
        public static readonly string ERROR_LOG_PATH = Path.Combine(LOG_DIR, ERROR_LOG_NAME + ".log");
        public const string CONSOLE_LOG_NAME = "console-debugger";
        public const string LOG_LAYOUT = "${longdate}|${uppercase:${level}}| ${message}";
        #endregion

        #endregion

        #endregion

        #endregion

        #region Constructors
        #endregion

        #region Methods

        public static void Initialize()
        {
            if (Initialized) return;
            Initialized = true;

            ConfigureLogger();
#if DEBUG
            // Register and Initialize the Console Debugger
            Trace.Listeners.Add(new NLogTraceListener());
            Debug.Listeners.Add(new NLogTraceListener());

            PresentationTraceSources.Refresh();
            PresentationTraceSources.DataBindingSource.Listeners.Add(new NLogTraceListener());
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Warning | SourceLevels.Error;
            
            ConsoleManager.Show();
            Log.Info("Welcome to the {0} Debugger", PRODUCT_NAME);
#endif

            CreateDirectories(Core.WORK_DIR, Core.TEMP_DIR, Core.LOG_DIR, Core.DATABASE_DIR);
            ClearDirectory(Core.TEMP_DIR);
        }

        /// <summary>
        /// Easy and safe way to create multiple directories. 
        /// </summary>
        /// <param name="directories">The set of directories to create</param>
        public static void CreateDirectories(params string[] directories)
        {
            if (directories == null || directories.Length <= 0) return;

            foreach (var directory in directories)
                try
                {
                    if (Directory.Exists(directory)) continue;

                    Directory.CreateDirectory(directory);
                    Log.Info("A new directory has been created ({0})", directory);
                }
                catch (Exception e)
                {
                    Log.Error("Error while creating directory {0} - {1}", directory, e);
                }
        }

        public static void ClearDirectory(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory)) return;

            foreach (var file in Directory.EnumerateFiles(directory, "*"))
                try { File.Delete(file); }
                catch (Exception e) { Log.Error("Failed to delete {0}\n", file, e); }
        }

        static void ConfigureLogger()
        {
            var config = new LoggingConfiguration();

#if DEBUG
            var debugConsole = new ColoredConsoleTarget()
            {
                Name = Core.CONSOLE_LOG_NAME,
                Layout = Core.LOG_LAYOUT,
                Header = $"{PRODUCT_NAME} Debugger"
            };

            var debugRule = new LoggingRule("*", LogLevel.Debug, debugConsole);
            config.LoggingRules.Add(debugRule);
#endif

            var errorFileTarget = new FileTarget()
            {
                Name = Core.ERROR_LOG_NAME,
                FileName = Core.ERROR_LOG_PATH,
                Layout = Core.LOG_LAYOUT
            };

            config.AddTarget(errorFileTarget);

            var errorRule = new LoggingRule("*", LogLevel.Error, errorFileTarget);
            config.LoggingRules.Add(errorRule);

            LogManager.Configuration = config;

            LogManager.ReconfigExistingLoggers();
        }

        #endregion
    }
}
