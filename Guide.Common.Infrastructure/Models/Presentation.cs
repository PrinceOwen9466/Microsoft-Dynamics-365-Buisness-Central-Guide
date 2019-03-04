using CommonServiceLocator;
using Guide.Common.Infrastructure.Extensions;
using Guide.Common.Infrastructure.Models.Interfaces;
using Guide.Common.Infrastructure.Resources.AttachedProperties;
using Guide.Common.Infrastructure.Resources.Controls;
using Guide.Common.Infrastructure.Services.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Unity;

namespace Guide.Common.Infrastructure.Models
{
    public class Presentation : BindableBase, IPresentable
    {
        #region Properties

        #region Bindables
        IView _view = null;
        public IView View
        {
            get => _view;
            set
            {
                SetProperty(ref _view, value);
                RaisePropertyChanged(nameof(CurrentPage));
            }
        }

        int _index = 0;
        public int Index { get => _index; set => SetProperty(ref _index, value); }

        public bool CanNext => Index < PageMap.Count - 1 && !IsBusy;
        public bool CanPrevious => Index > 0 && !IsBusy;
        public int Total => PageMap.Count;

        //bool _isBusy = false;
        public bool IsBusy = false; //{ get => _isBusy; set => SetProperty(ref _isBusy, value); }

        public ObservableCollection<ISection> Sections { get; } = new ObservableCollection<ISection>();

        public PresentationControl CurrentPage => (PresentationControl)View;
        #endregion

        #region Internals
        bool Initialized { get; set; }
        IUnityContainer Container { get; }
        CancellationTokenSource CancellationSource { get; } = new CancellationTokenSource();
        List<Reference> PageMap { get; } = new List<Reference>();
        IView[] Pages { get; } = new IView[5];
        Type ContentType { get; set; }
        #endregion

        #endregion

        #region Constructors
        public Presentation()
        {
            Container = ServiceLocator.Current.TryResolve<IUnityContainer>();
        }
        #endregion

        #region Methods

        
        #region IPresentable Implementation
        public async void Initialize(Type content, Type sectionPage)
        {
            ContentType = content;
            foreach (var field in content.GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                var property = field.GetValue(null);

                if (property is ISection)
                {
                    if (property is Section)
                        PageMap.Add(new Reference((Section)property, sectionPage, true));
                    foreach (var mapElement in ((ISection)property).PageMap)
                        PageMap.Add(mapElement);

                    Sections.Add((ISection)property);   
                }
                else throw new InvalidOperationException("Attempted to initialize a presentation content with a class that does not implement ISection");
            }

            for (int i = 2; i < Pages.Length; i++)
            {
                var view = await ResolveReference(PageMap[i-2]);

                if (!(view is IView)) throw new Exception("Page does not implement IView");

                Pages[i] = (IView)view;
                if (i == 2) Application.Current.Dispatcher.Invoke(() => View = Pages[i]);
            }

            AnalyzePages();
        }

        public async Task Next()
        {
            if (!CanNext) return;
            IsBusy = true;

            Core.Log.Debug("");
            Core.Log.Debug("Next Called!!!");
            Index++;
            ShiftArrayForward(Pages);

            int mainIndex = Pages.Length / 2;
            if (Pages[mainIndex] == null)
                Pages[mainIndex] = await ResolveReference(PageMap[Index]);
            await Application.Current.Dispatcher.InvokeAsync(() => View = Pages[mainIndex]);

            EventHandler openedHandler = null;
            View.Opened += openedHandler = async (s, e) =>
            {
                View.Opened -= openedHandler;
                Application.Current.Dispatcher.WaitTillIdle(200);
                await LoadPages();
                // await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => LoadPages()));
                IsBusy = false;
            };
        }

        public async Task Previous()
        {
            if (!CanPrevious) return;
            IsBusy = true;

            
            Index--;
            ShiftArrayBackwards(Pages);
            int mainIndex = Pages.Length / 2;
            if (Pages[mainIndex] == null)
                Pages[mainIndex] = await ResolveReference(PageMap[Index]);

            await Application.Current.Dispatcher.InvokeAsync(() => View = Pages[mainIndex]);

            EventHandler openedHandler = null;
            View.Opened += openedHandler = async (s, e) =>
            {
                View.Opened -= openedHandler;
                Application.Current.Dispatcher.WaitTillIdle(200);
                var watch = Stopwatch.StartNew();
                await LoadPages();
                //await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => LoadPages()));
                watch.Stop();
                Core.Log.Debug($"Took {watch.Elapsed} to LoadPages");
                IsBusy = false;
            };
        }

        public async void OpenSection(ISection section)
        {
            if (section == null || IsBusy) return;
            IsBusy = true;

            // Clear the pages
            for (int i = 0; i < Pages.Length; i++) Pages[i] = null;

            int mainIndex = Pages.Length / 2;
            Index = PageMap.FindIndex(p => p.IsSecton && p.TargetSection.Index == section.Index);
            Pages[mainIndex] = await ResolveReference(PageMap[Index]);

            await Application.Current.Dispatcher.InvokeAsync(() => View = Pages[mainIndex]);

            EventHandler openedHandler = null;
            View.Opened += openedHandler = async (s, e) =>
            {
                View.Opened -= openedHandler;
                Application.Current.Dispatcher.WaitTillIdle(200);
                var watch = Stopwatch.StartNew();
                await LoadPages();
                watch.Stop();
                Core.Log.Debug($"Took {watch.Elapsed} to LoadPages");
                IsBusy = false;
            };
        }

        public async void GoToPage(Page page)
        {
            PresentationControl control = await ResolveControl(page.PageReference);
            RippleControlButton button = null;

            foreach (var id in page.IterateIDs().Skip(1))
            {
                button = control.FindLogicalChildren<RippleControlButton>(false).
                    FirstOrDefault(b => ReferenceKey.GetKey(b) == id);
                button.Activate();
            }
        }
        #endregion

        #region Tasks
        async Task<PresentationControl> ResolveControl(Type controlType, object context = null)
        {
            try
            {
                PresentationControl control = null;
                await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    try
                    {
                        control = (PresentationControl)Container.Resolve(controlType);
                    }
                    catch (Exception cex)
                    {
                        Core.Log.Debug($"An error occured while resolving control \n{cex}");
                    }
                }));

                if (control != null && context != null)
                    control.DataContext = context;
                return control;
            }
            catch (Exception ex)
            {
                Core.Log.Error($"An error occured while resolving a control\n{ex}");
                return null;
            }
        }
        #endregion

        async Task LoadPages()
        {
            try
            {
                int mid = Pages.Length / 2;


                for (int i = 0; i <= mid; i++)
                {
                    int f = mid + i;
                    int b = mid - i;

                    Core.Log.Debug($"Repelenishing {b} -- {f} ---- Index: {Index}");
                    Core.Log.Debug($"Backward: {b >= 0 && Pages[b] == null && Index - i >= 0}");
                    Core.Log.Debug($"Forward: {f < Pages.Length && Pages[f] == null && Index + i < PageMap.Count}");

                    if (f < Pages.Length && Pages[f] == null && Index + i < PageMap.Count)
                    {
                        Core.Log.Debug($"Forward to page map ref {PageMap[Index + i].TargetPage}");
                        Pages[f] = await ResolveReference(PageMap[Index + i]);
                        Core.Log.Debug($"Replenished {f} -- {Pages[f]} -- Index: {Index} -- i: {i}");
                    }
                    if (b >= 0 && Pages[b] == null && Index - i >= 0)
                    {
                        Pages[b] = await ResolveReference(PageMap[Index - i]);
                        Core.Log.Debug($"Replenished {b} -- {Pages[b]} -- Index: {Index} -- i: {i}");
                    }
                }

                Core.Log.Debug("");
                Core.Log.Debug("-- Slots --");
                foreach (var page in Pages)
                    Core.Log.Warn(page);
                Core.Log.Debug("-- End Of Slots --");

                Core.Log.Debug("Load Complete");
            }
            catch (Exception ex)
            {
                Core.Log.Error($"An error occured while loading pages\n{ex}");
            }
            
        }

        async Task<IView> ResolveReference(Reference reference)
        {
            try
            {
                IView view = null;
                if (reference.IsSecton) view = (IView)await ResolveControl(reference.TargetPage, reference.TargetSection);
                else view = (IView)await ResolveControl(reference.TargetPage);

                view.Section = reference.TargetSection;
                Core.Log.Debug(view.Section);
                return view;
            }
            catch (Exception ex)
            {
                Core.Log.Error($"An error occured while resolving references\n{ex}");
                return null;
            }
        }

        void ShiftArrayForward(object[] array)
        {
            for (int i = 0; i < array.Length; i++)
                if (i + 1 < array.Length)
                    array[i] = array[i + 1];
                else array[i] = null;

            Core.Log.Debug("");
            Core.Log.Debug("-- Pages Shifted Forwards --");
            foreach (var page in Pages)
                Core.Log.Warn(page);
            Core.Log.Debug("-- End Of shift --");
        }

        void ShiftArrayBackwards(object[] array)
        {
            for (int i = array.Length - 1; i >= 0; i--)
                if (i - 1 >= 0)
                    array[i] = array[i - 1];
                else array[i] = null;

            Core.Log.Debug("");
            Core.Log.Debug("-- Pages Shifted Backwards --");
            foreach (var page in Pages)
                Core.Log.Warn(page);
            Core.Log.Debug("-- End Of shift --");
        }
        
        async void AnalyzePages()
        {
            var watch = Stopwatch.StartNew();
            Section currentSection = null;

            Core.Log.Debug("");
            Core.Log.Debug("Text Data Coming Up...");
            foreach (var page in PageMap)
            {
                if (currentSection == null || currentSection.Name != page.TargetSection.Name)
                    currentSection = page.TargetSection;

                var reference = (DependencyObject)await ResolveReference(page);

                //currentSection.TotalPages += UIHelper.FindAllRippleButtons(reference).Count();
                DisplayText(reference);
            }
            watch.Stop();
            Core.Log.Debug($"Finished in {watch.Elapsed}");
        }

        IEnumerable<Page> Analyze()
        {
            return null;
            /*
            var watch = Stopwatch.StartNew();
            Section currentSection = null;

            foreach (var page in PageMap)
            {
                var reference = (DependencyObject)await ResolveReference(page);
                
                foreach (var r in reference.FindLogicalChildren<RippleControlButton>())
                {
                    if (r.TransitionContent is PresentationControl)
                    {
                        ((PresentationControl)r.TransitionContent).
                    }
                }


                foreach (var p in reference.FindAllPages())
                {
                    string content = string.Empty;
                    foreach (var textBlock in p.FindLogicalChildren<TextBlock>())
                        content += textBlock.Text + "\n";
                    string title = p.Title;
                    
                    //string content = p.FindLogicalChildren<TextBlock>().
                    //yield return new Page() { I}
                }
            }

            watch.Stop();
            */
        }

        void DisplayText(DependencyObject obj)
        {
            foreach (var block in UIHelper.FindLogicalChildren<TextBlock>(obj))
                Core.Log.Debug($"{obj} : \n{block.Text}");
        }
        #endregion
    }
}

