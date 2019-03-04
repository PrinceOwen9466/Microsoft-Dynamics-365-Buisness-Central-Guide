using Guide.Common.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Guide.Common.Infrastructure.Resources.Behaviors
{
    public class ClickThroughBehavior : Behavior<FrameworkElement>
    {
        #region Properties

        #region Internals
        FrameworkElement HitElement => GetHit();
        Window Window { get; set; }
        #endregion

        #endregion

        #region Methods

        #region Overrides
        protected override void OnAttached()
        {
            base.OnAttached();

            Window = AssociatedObject.FindParent<Window>();

            Window.MouseMove += (s, e) =>
            {
                if (IsHit(AssociatedObject)) RaiseMouseEvent(AssociatedObject, e);
            };


            /*
            AssociatedObject.MouseEnter += (s, e) => RaiseMouseEvent(HitElement, e);
            AssociatedObject.MouseLeave += (s, e) => RaiseMouseEvent(HitElement, e);
            AssociatedObject.MouseMove += (s, e) => RaiseMouseEvent(HitElement, e);
            AssociatedObject.GotMouseCapture += (s, e) => RaiseMouseEvent(HitElement, e);
            AssociatedObject.LostMouseCapture += (s, e) => RaiseMouseEvent(HitElement, e);
            AssociatedObject.PreviewMouseMove += (s, e) => RaiseMouseEvent(HitElement, e);
            */


            //AssociatedObject.Mouse
        }
        #endregion

        bool IsHit(FrameworkElement element)
        {
            Window window = AssociatedObject.FindParent<Window>();
            Point point = Mouse.GetPosition(window);
            bool hit = false;

            VisualTreeHelper.HitTest(window, null, (result) =>
            {
                hit = hit || result.VisualHit == AssociatedObject;
                if (hit) return HitTestResultBehavior.Stop;
                return HitTestResultBehavior.Continue;
                /*
                if (!(result.VisualHit is FrameworkElement)) return HitTestResultBehavior.Continue;

                element = result.VisualHit as FrameworkElement;

                Core.Log.Debug(element);
                if (element == AssociatedObject || element == null || !element.IsVisible || !element.IsHitTestVisible) return HitTestResultBehavior.Continue;
                return HitTestResultBehavior.Stop;
                */
            }, new PointHitTestParameters(point));

            //Core.Log.Debug(element);

            return hit;
        }

        FrameworkElement GetHit()
        {
            Window window = AssociatedObject.FindParent<Window>();
            Point point = Mouse.GetPosition(window);
            FrameworkElement element = null;

            Core.Log.Debug("");
            Core.Log.Debug("");
            Core.Log.Debug("");
            VisualTreeHelper.HitTest(window, null, (result) =>
            {
                if (!(result.VisualHit is FrameworkElement)) return HitTestResultBehavior.Continue;

                element = result.VisualHit as FrameworkElement;

                Core.Log.Debug(element);
                if (element == AssociatedObject || element == null || !element.IsVisible || !element.IsHitTestVisible) return HitTestResultBehavior.Continue;
                return HitTestResultBehavior.Stop;
            }, new PointHitTestParameters(point));

            //Core.Log.Debug(element);
            
            return element;
        }

        FrameworkElement GetHit(MouseEventArgs e)
        {
            Window window = AssociatedObject.FindParent<Window>();
            Point point = e.GetPosition(window);
            FrameworkElement element = null;

            VisualTreeHelper.HitTest(window, null, (result) =>
            {
                if (!(result.VisualHit is FrameworkElement)) return HitTestResultBehavior.Continue;

                element = result.VisualHit as FrameworkElement;
                if (element == AssociatedObject || element == null || !element.IsVisible) return HitTestResultBehavior.Continue;
                return HitTestResultBehavior.Stop;
            }, new PointHitTestParameters(point));

            Core.Log.Debug(element);
            return element;
        }

        void RaiseMouseEvent(FrameworkElement element, MouseEventArgs e)
        {
            if (e.Source != AssociatedObject) return;

            element.RaiseEvent(new MouseEventArgs(e.MouseDevice, e.Timestamp, e.StylusDevice)
            {
                RoutedEvent = e.RoutedEvent,
                Source = element,
                Handled = false
            });
        }
        #endregion
    }
}
