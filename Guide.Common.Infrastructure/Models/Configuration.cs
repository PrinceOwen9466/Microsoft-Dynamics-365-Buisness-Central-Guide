using CommonServiceLocator;
using Guide.Common.Infrastructure.Models.Interfaces;
using Guide.Common.Infrastructure.Services.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Unity;

namespace Guide.Common.Infrastructure.Models
{
    public class Configuration : BindableBase
    {
        #region Properties

        #region Event Handlers
        
        #endregion

        public static Configuration DefaultConfiguration = new Configuration();

        bool _contentAnalyzed = false;
        public bool ContentAnalyzed { get => _contentAnalyzed; set => SetProperty(ref _contentAnalyzed, value); }

        Page _recentPage = null;
        public Page RecentPage { get => _recentPage; set => SetProperty(ref _recentPage, value); }

        ObservableCollection<Page> _readPages = new ObservableCollection<Page>();
        public ObservableCollection<Page> ReadPages { get => _readPages; set => SetProperty(ref _readPages, value); }

        
        double _recentProgress = .0;
        [XmlIgnore]
        public double RecentProgress { get => _recentProgress; set => SetProperty(ref _recentProgress, value); }
        [XmlIgnore]
        public ObservableCollection<ProgressWrapper> Progress { get; } = new ObservableCollection<ProgressWrapper>();

        public ObservableCollection<Section> Sections { get; set; } = new ObservableCollection<Section>();
        #endregion

        #region Constructors
        public Configuration() { }
        public Configuration(Configuration configuration)
        {
            RecentPage = configuration.RecentPage;
            for (int i = 0; i < configuration.ReadPages.Count; i++)
                ReadPages.Add(new Page(configuration.ReadPages[i]));

            RefreshProgress();
        }
        #endregion

        #region Methods

        public void RefreshProgress()
        {
            /*
            IPresenter presenter = ServiceLocator.Current.GetInstance<IPresenter>();
             if (presenter.Content == null) return;
            var sections = presenter.Content.Sections;

            if (RecentPage == null)
                RecentPage = presenter.Content.Sections[0].PageMap[0];

            Progress.Clear();
            for (int i = 0; i < sections.Count; i++)
            {
                double count = sections[i].PageMap.Count(r => ReadPages.FirstOrDefault(x => x.TargetPageAssemblyName == r.TargetPageAssemblyName) != null);

                Progress.Add(new ProgressWrapper
                {
                    Progress = (count / sections[i].TotalPages) * 100,
                    Section = sections[i]
                });
            }

            var first = Progress.FirstOrDefault(p => p.Section.Name == RecentPage.TargetSection.Name);
            RecentProgress = first == null ? 0 : first.Progress;
            */
        }
        #endregion
    }
}
