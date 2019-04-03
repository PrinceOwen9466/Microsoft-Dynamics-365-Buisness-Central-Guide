using Guide.Common.Infrastructure.Extensions;
using Guide.Common.Infrastructure.Resources.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Guide.Common.Infrastructure.Resources.AttachedProperties
{
    public enum HitMode { Single, Multiple }

    public static class Hits
    {
        #region Properties

        public static HitMode Mode { get; set; } = HitMode.Single;

        #region Attached
        public static bool GetRecordHits(DependencyObject obj)
        {
            return (bool)obj.GetValue(RecordHitsProperty);
        }

        public static void SetRecordHits(DependencyObject obj, bool value)
        {
            obj.SetValue(RecordHitsProperty, value);
        }
        public static readonly DependencyProperty RecordHitsProperty =
            DependencyProperty.RegisterAttached("RecordHits", typeof(bool), typeof(Hits), new UIPropertyMetadata(false, OnPropertyChanged));


        public static bool GetMouseOver(DependencyObject obj)
        {
            return (bool)obj.GetValue(MouseOverProperty);
        }

        public static void SetMouseOver(DependencyObject obj, bool value)
        {
            if (GetMouseOver(obj) != value)
                obj.SetValue(MouseOverProperty, value);
        }
        public static readonly DependencyProperty MouseOverProperty =
            DependencyProperty.RegisterAttached("MouseOver", typeof(bool), typeof(Hits), new UIPropertyMetadata(false));



        public static FrameworkElement GetHitElement(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(HitElementProperty);
        }

        public static void SetHitElement(DependencyObject obj, FrameworkElement value)
        {
            if (obj is FrameworkElement)
            {
                var el = (FrameworkElement)obj;

                if (el.Name == "DemoGrid")
                {
                    Core.Log.Debug(value);
                }
            }
            obj.SetValue(HitElementProperty, value);
        }

        public static readonly DependencyProperty HitElementProperty =
            DependencyProperty.RegisterAttached("HitElement", typeof(FrameworkElement), typeof(Hits), new UIPropertyMetadata(null));






        #endregion

        #region Events
        static event MouseEventHandler MouseMoveEventHandler;
        static event MouseEventHandler MouseLeaveEventHandler;
        #endregion

        #region Internals
        static List<FrameworkElement> Registered { get; } = new List<FrameworkElement>();
        static Window Window { get; set; }
        static Dictionary<FrameworkElement, List<FrameworkElement>> Registry { get; } = new Dictionary<FrameworkElement, List<FrameworkElement>>();
        #endregion

        #endregion

        #region Constructors
        static Hits()
        {
            // Application.Current.MainWindow.Closing += (s, e) => Win32.RemoveMouseHook();
            // Win32.SetMouseHook();
          


            MouseMoveEventHandler = (s, ev) =>
            {
                var con = s as FrameworkElement;
                foreach (var el in Registry[con])
                {
                    if (el.Name == "DemoGrid")
                    {
                        var p = Mouse.GetPosition(con);
                        Console.WriteLine($"DemoGrid -- ({p.X}, {p.Y}) -- Container {con}");
                        var hitElement = GetHitElement(el);
                    }
                    SetMouseOver(el, IsHit(GetHitElement(el), con));
                }
            };

            MouseLeaveEventHandler = (s, e) =>
            {
                var con = s as FrameworkElement;
                foreach (var el in Registry[con])
                    SetMouseOver(el, false);
            };
        }
        #endregion


        #region Methods

        static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            switch (Mode)
            {
                case HitMode.Single:
                    if (e.Property == RecordHitsProperty)
                    {
                        if (!(d is FrameworkElement)) return;
                        FrameworkElement el = d as FrameworkElement;

                        if (GetHitElement(d) == null)
                            SetHitElement(d, (FrameworkElement)d);

                        el.Unloaded += (s, ev) => Registered.Remove(el);
                        
                        if ((bool)e.NewValue) Registered.Add(el);
                        else Registered.Remove(el);
                    }

                    InitializeEvents();
                    break;

                case HitMode.Multiple:
                    if (e.Property == RecordHitsProperty)
                    {
                        if (GetHitElement(d) == null) SetHitElement(d, (FrameworkElement)d);
                        if ((bool)e.NewValue) RegisterElement(d);
                        else UnregisterElement(d);
                    }
                    break;
            }

          

            /*
            if (e.Property == RecordHitsProperty)
            {
                if ((bool)e.NewValue) Registered.Add(d);
                else Registered.Remove(d);
            }

            InitializeEvents();
            */
        }

        #region Single Mode Implementation
        static void InitializeEvents()
        {
            if (Window != null) return;

            Window = Application.Current.MainWindow;
            if (Window == null) return;

            /*
            Win32.MouseAction += (s, ev) =>
            {
                if (Window == null) return;
                Point point = ev.Position;
                Point rel = Window.PointFromScreen(point);
                if (rel.X < 0 || rel.Y < 0) return;
                
                foreach (var element in Registered)
                    SetMouseOver(element, IsHit(GetHitElement(element), rel));
            };
            */

            Window.MouseMove += (s, e) =>
            {
                foreach (var element in Registered)
                    SetMouseOver(element, IsHit(GetHitElement(element)));
            };

            Window.MouseLeave += (s, e) =>
            {
                foreach (var element in Registered)
                    if (element is FrameworkElement)
                        SetMouseOver(element, false);
            };
        }

        static bool IsHit(FrameworkElement element)
        {
            Window window = Window;
            Point point = Mouse.GetPosition(window);
            bool hit = element.TransformToVisual(window).TransformBounds(new Rect(element.RenderSize)).Contains(point);
            if (element.DataContext is IHitableContext) ((IHitableContext)element.DataContext).MouseHit = hit;
            /*
            VisualTreeHelper.HitTest(window, null, (result) =>
            {
                hit = result.VisualHit == element;
                Core.Log.Debug(result.VisualHit + $"-   {hit}");
                if (hit) return HitTestResultBehavior.Stop;
                return HitTestResultBehavior.Continue;
            }, new PointHitTestParameters(point));
            */
            return hit;
        }

        static bool IsHit(FrameworkElement element, Point point)
        {
            Window window = Window;
            bool hit = element.TransformToVisual(window).TransformBounds(new Rect(element.RenderSize)).Contains(point);
            if (element.DataContext is IHitableContext) ((IHitableContext)element.DataContext).MouseHit = hit;
            return hit;
        }
        #endregion

        static bool IsHit(FrameworkElement element, FrameworkElement container)
        {
            Point point = Mouse.GetPosition(container);
            bool hit = element.TransformToVisual(container).TransformBounds(new Rect(element.RenderSize)).Contains(point);
            if (element.DataContext is IHitableContext) ((IHitableContext)element.DataContext).MouseHit = hit;
            return hit;
        }


        static void RegisterElement(DependencyObject @object)
        {
            if (!(@object is FrameworkElement)) throw new InvalidOperationException("Only framework elements can be used for hits!");

            var element = @object as FrameworkElement;

            if (!element.IsLoaded)
            {
                RoutedEventHandler loaded = null;
                element.Loaded += loaded = (s, e) =>
                {
                    element.Loaded -= loaded;
                    RegisterElement(element);
                };
                return;
            }


            var container = element.FindAlphaParent();

            if (container == null)
                throw new InvalidOperationException("The window of the registered element was not found");

            List<FrameworkElement> elementList = null;

            if (Registry.TryGetValue(container, out elementList))
                elementList.Add(element);
            else
            {
                elementList = new List<FrameworkElement>();
                elementList.Add(element);
                Registry.Add(container, elementList);
            }

            container.MouseMove += MouseMoveEventHandler;
            container.MouseLeave += MouseLeaveEventHandler;
        }

        static void UnregisterElement(DependencyObject @object)
        {
            if (!(@object is FrameworkElement)) throw new InvalidOperationException("Only framework elements can be used for hits!");

            var element = @object as FrameworkElement;
            var container = element.FindAlphaParent();

            if (container == null)
                throw new InvalidOperationException("The container of the registered element could not be found.");

            if (!Registry.TryGetValue(container, out var elementList)) return;

            elementList.Remove(element);
            if (elementList.Count <= 0) Registry.Remove(container);

            container.MouseMove -= MouseMoveEventHandler;
            container.MouseLeave -= MouseLeaveEventHandler;
        }
        #endregion
    }
}
