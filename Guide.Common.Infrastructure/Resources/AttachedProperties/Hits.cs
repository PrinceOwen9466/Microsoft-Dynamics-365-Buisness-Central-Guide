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
    public static class Hits
    {
        #region Properties

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
            obj.SetValue(MouseOverProperty, value);
        }
        public static readonly DependencyProperty MouseOverProperty =
            DependencyProperty.RegisterAttached("MouseOver", typeof(bool), typeof(Hits), new PropertyMetadata(false));
        #endregion

        #region Internals
        static List<DependencyObject> Registered { get; } = new List<DependencyObject>();
        static Window Window { get; set; }
        #endregion

        #endregion

        #region Methods

        static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == RecordHitsProperty)
            {
                if ((bool)e.NewValue) Registered.Add(d);
                else Registered.Remove(d);
            }

            InitializeEvents();
        }

        static void InitializeEvents()
        {
            if (Window != null) return;

            Window = Application.Current.MainWindow;
            if (Window == null) return;
            Window.MouseMove += (s, e) =>
            {
                foreach (var element in Registered)
                    if (element is FrameworkElement)
                        SetMouseOver(element, IsHit((FrameworkElement)element));
            };
        }

        static bool IsHit(FrameworkElement element)
        {
            Window window = Window;
            Point point = Mouse.GetPosition(window);
            bool hit = element.TransformToVisual(window).TransformBounds(new Rect(element.RenderSize)).Contains(point);
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
    

        #endregion
    }
}
