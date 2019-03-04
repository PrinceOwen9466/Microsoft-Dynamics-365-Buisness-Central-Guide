using Guide.Common.Infrastructure.Extensions;
using Guide.Common.Infrastructure.Models;
using Guide.Common.Infrastructure.Services.Interfaces;
using Prism.Logging;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Guide.Common.Infrastructure.Services
{
    public class XMLConfigurationManager : BindableBase, IConfigurationManager
    {
        #region Properties

        #region Bindables

        Configuration _currentConfiguration = null;
        public Configuration CurrentConfiguration { get => _currentConfiguration; private set => SetProperty(ref _currentConfiguration, value); }
        #endregion

        #region Services
        ILoggerFacade Logger { get; }
        IPresenter Presenter { get; }
        #endregion

        #region Internals
        bool PresenterContentRegistered { get; }
        #endregion

        #endregion

        #region Constructors
        public XMLConfigurationManager(ILoggerFacade logger, IPresenter presenter)
        {
            Logger = logger;
            Presenter = presenter;
            LoadConfiguration();

            RegisterPresenter();
        }
        #endregion

        #region Methods

        #region IConfigurationManager Implementation
        public bool Save(Configuration configuration = null)
        {
            if (configuration == null)
                configuration = CurrentConfiguration;
            string destination = Core.CONFIGURATION_PATH;

            try
            {
                using (var stream = new StreamWriter(destination))
                    new XmlSerializer(typeof(Configuration)).Serialize(stream, configuration);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured while attempting to save configuration. \n {0}", ex);
                return false;
            }
        }
        #endregion

        void LoadConfiguration()
        {
            Configuration configuration = null;
            try
            {
                using (var stream = new FileStream(Core.CONFIGURATION_PATH, FileMode.Open, FileAccess.Read))
                    configuration = (Configuration)new XmlSerializer(typeof(Configuration)).Deserialize(stream);

                CurrentConfiguration = new Configuration(configuration);
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured while attempting to load configuration...\n{0}", ex);
                CurrentConfiguration = new Configuration(Configuration.DefaultConfiguration);
                Save();
            }
        }

        #region Registerations
        void RegisterPresenter()
        {
            /*
            if (Presenter is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)Presenter).PropertyChanged += (s, e) =>
                {
                    if (!PresenterContentRegistered && e.PropertyName == nameof(Presenter.Content))
                        if (Presenter.Content is INotifyPropertyChanged)
                        {
                            var element = ((INotifyPropertyChanged)Presenter.Content);
                            element.PropertyChanged += (se, ev) =>
                            {
                                switch (ev.PropertyName)
                                {
                                    case "View":
                                        Reference reference = new Reference
                                        {
                                            TargetPage = Presenter.Content.View.GetType(),
                                            TargetSection = (Section)Presenter.Content.View.Section
                                        };

                                        var instance = CurrentConfiguration.ReadPages.FirstOrDefault(r => r.TargetPageAssemblyName == reference.TargetPageAssemblyName);
                                        CurrentConfiguration.RecentPage = reference;

                                        if (instance == null)
                                            CurrentConfiguration.ReadPages.Add(reference);
                                        Save();
                                        CurrentConfiguration.RefreshProgress();
                                        break;
                                }
                            };
                        }
                };
            }
            */
        }
        #endregion


        #endregion
    }
}
