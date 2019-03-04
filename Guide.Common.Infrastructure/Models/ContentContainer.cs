using CommonServiceLocator;
using Guide.Common.Infrastructure.Models.Interfaces;
using Guide.Common.Infrastructure.Resources.Collections;
using Guide.Common.Infrastructure.Resources.Controls;
using Guide.Common.Infrastructure.Services.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace Guide.Common.Infrastructure.Models
{
    /*
    public class ContentContainer : BindableBase, IPresentable
    {
        #region Properties

        #region Statics
        const string SectionPresenterName = "SectionPresenter";
        #endregion


        public ObservableCollection<Section> Sections { get; private set; }

        public PresentationControl[] Pages { get; private set; }


        Section _currentSection = null;
        public Section CurrentSection
        {
            get => _currentSection;
            set => SetProperty(ref _currentSection, value);
        }

        public int CurrentSectionIndex
        {
            get
            {
                int index = 0;
                int sectionIndex = 0;
                while (index < Sections.Count && Sections[index] != CurrentSection)
                    sectionIndex += Sections[index++].ContentCount + 1;
                return sectionIndex;
            }
        }

        PresentationControl _currentPage = null;
        public PresentationControl CurrentPage
        {
            get => _currentPage;
            set
            {
                SetProperty(ref _currentPage, value);
                RaisePropertyChanged(nameof(View));
            }
        }


        public int SectionIndex { get; set; }


        int _totalPageIndex = 1;
        public int TotalPageIndex
        {
            get => _totalPageIndex;
            set => SetProperty(ref _totalPageIndex, value);
        }

        


        int _pageIndex = 1;
        public int PageIndex
        {
            get => _pageIndex;
            set
            {
                SetProperty(ref _pageIndex, value);
                Core.Log.Debug($"Page: {PageIndex}");
            }
        }

        public int PageTotal => Pages.Length;

        public bool CanNext => CurrentSectionIndex + PageIndex < PageTotal;
        public bool CanPrevious => CurrentSectionIndex + PageIndex > 1;

        #region Internals
        bool Initialized { get; set; }
        IUnityContainer Container { get; }
        Type SectionPage { get; set; }
        PresentationControl SectionPresenter { get; set; }

        int ZTotalPageIndex => TotalPageIndex - 1;


        public object View => CurrentPage;

        public int Index => throw new NotImplementedException();

        public int Total => throw new NotImplementedException();

        IView IPresentable.View => throw new NotImplementedException();
        #endregion

        #endregion

        #region Constructors
        public ContentContainer()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Sections = new AsyncObservableCollection<Section>();
            });

            Container = ServiceLocator.Current.TryResolve<IUnityContainer>();
        }
        #endregion

        #region Methods

        public void AddSection(Section section)
        {
            Sections.Add(section);

            if (CurrentSection == null)
            {
                CurrentSection = Sections[0];
                CurrentPage = (PresentationControl)Container.Resolve(Sections[0].PageMap[0].TargetPage);
            }
        }

        public void Initialize(Type sectionPage)
        {
            SectionPresenter = (PresentationControl)Container.Resolve(sectionPage);
            SectionPresenter.Name = SectionPresenterName;
            SectionPresenter.DataContext = CurrentSection;

            int pageTotal = 0;
            for (int i = 0; i < Sections.Count; i++)
                pageTotal += Sections[i].ContentCount + 1;

            Pages = new PresentationControl[pageTotal];

            int j = 0;
            for (int i = 0; i < Sections.Count; i++)
            {
                Core.Log.Debug($"{j + 1},");
                Pages[j] = SectionPresenter;
                j += Sections[i].ContentCount + 1;
                
            }
            

            Initialized = true;
            SectionPage = sectionPage;
            

            CurrentPage = SectionPresenter;

            SectionIndex = CurrentSection.Index;
        }

        public void Previous()
        {
            if (!CanPrevious) return;


            //var page = Pages[ZTotalPageIndex - 1];
            
            
            //if (page is PresentationControl && page.Name == SectionPresenterName)



            if (PageIndex <= 1)
            {
                CurrentSection = Sections[SectionIndex - 2];

                PageIndex = CurrentSection.ContentCount + 1;
                SectionIndex = CurrentSection.Index;

                SectionPresenter = (PresentationControl)Container.Resolve(SectionPage);
                SectionPresenter.DataContext = CurrentSection;

                if (PageIndex == 1)
                {
                    var sectionPage = (PresentationControl)Container.Resolve(SectionPage);
                    sectionPage.DataContext = CurrentSection;
                    CurrentPage = sectionPage;
                }
                else CurrentPage = (PresentationControl)Container.Resolve(CurrentSection.PageMap[PageIndex - 2].TargetPage);
            }
            else if (PageIndex == 2)
            {
                CurrentPage.DataContext = CurrentSection;
                CurrentPage = (PresentationControl)SectionPresenter;
            }
            else CurrentPage = (PresentationControl)Container.Resolve(CurrentSection.PageMap[PageIndex - 3].TargetPage);
            
            if (PageIndex > 1)
                PageIndex--;
        }

        public void Next()
        {
            if (!CanNext) return;

            int sectionEnd = CurrentSection.ContentCount;


            bool newSection = PageIndex > sectionEnd;
            if (newSection)
            {
                CurrentSection = Sections[SectionIndex];

                PageIndex = 1;
                SectionIndex = CurrentSection.Index;

                SectionPresenter = (PresentationControl)Container.Resolve(SectionPage);
                SectionPresenter.DataContext = CurrentSection;
                CurrentPage = SectionPresenter;
            }
            else CurrentPage = (PresentationControl)Container.Resolve(CurrentSection.PageMap[PageIndex-1].TargetPage);

            if (!newSection)
                PageIndex++;
        }

        public void OpenSection(Section section)
        {
            if (!Sections.Contains(section)) return;

            CurrentSection = section;

            PageIndex = 1;
            SectionIndex = CurrentSection.Index;

            SectionPresenter = (PresentationControl)Container.Resolve(SectionPage);
            SectionPresenter.DataContext = CurrentSection;
            CurrentPage = SectionPresenter;
        }

        public void OpenSection(ISection section)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    */
}
