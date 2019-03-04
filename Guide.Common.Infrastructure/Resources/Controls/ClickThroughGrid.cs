using Guide.Common.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    public class ClickThroughGrid : Grid
    {
        #region Properties

        #region Internals
        List<object> Hits { get; } = new List<object>();
        #endregion

        #endregion

        #region Constructors
        public ClickThroughGrid()
        {
            /*
            MouseMove += (s, e) =>
            {
                var parent = this.FindParent<Panel>();
                //Core.Log.Debug(parent);
                if (parent != null)
                parent.RaiseEvent(e);
            };

            MouseEnter += (s, e) =>
            {
                var parent = this.FindParent<Panel>();
                //Core.Log.Debug(parent);
                if (parent != null)
                    parent.RaiseEvent(e);
            };
            */
        }
        #endregion

        #region Methods

        #region Overrides
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            //base.OnMouseDown(e);
            e.Handled = false;

            var point = e.GetPosition(this.FindParent<Window>());
            VisualTreeHelper.HitTest(this.FindParent<Window>(), null/*new HitTestFilterCallback((element) =>
            {
                if (element == this) return HitTestFilterBehavior.Continue;
                else if (element is UIElement && ((UIElement)element).IsHitTestVisible)
                    return HitTestFilterBehavior.Continue;
                else return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
            })*/, (result) =>
            {
                string name = string.Empty;
                if (result.VisualHit is FrameworkElement)
                    name = ((FrameworkElement)result.VisualHit).GetValue(FrameworkElement.NameProperty) as string;
                Core.Log.Debug($"{result} -- {result.VisualHit} -- [{name}]");
                Hits.Add(result.VisualHit);

                if (!(result.VisualHit is FrameworkElement)) return HitTestResultBehavior.Continue;

                var element = (FrameworkElement)result.VisualHit;

                if (element != this && element.IsVisible)
                {
                    element.RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton, e.StylusDevice)
                    {
                        RoutedEvent = MouseDownEvent,
                        Source = element
                    });
                    return HitTestResultBehavior.Stop;
                }

                /*
                if (result.VisualHit is SvgContainer)
                {
                    var container = result.VisualHit as SvgContainer;

                    var button = container.FindParent<Button>();
                    if (button == null) return HitTestResultBehavior.Continue;

                    button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));


                    *
                    MouseButtonEventArgs newargs = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton, e.StylusDevice)
                    {
                        RoutedEvent = MouseDownEvent,
                        Source = container
                    };
                    container.RaiseEvent(newargs);
                    *
                }*/

                return HitTestResultBehavior.Continue;
            }, new PointHitTestParameters(point));

            /*
            Core.Log.Debug("\nParents");
            this.FindParent<Window>(true);
            Core.Log.Debug("\nChildren");
            this.FindChild<Window>(true);
            var parent = this.FindParent<FrameworkElement>();

            if (parent == null) return;
            MouseButtonEventArgs newarg = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton, e.StylusDevice)
            {
                RoutedEvent = MouseDownEvent, Source = this
            };
            parent.RaiseEvent(newarg);
            */
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Hits.Clear();
            var point = e.GetPosition(this);
            /*
            VisualTreeHelper.HitTest(this, new HitTestFilterCallback((element) =>
            {
                if (element == this) return HitTestFilterBehavior.Continue;
                else if (element is UIElement && ((UIElement)element).IsHitTestVisible)
                    return HitTestFilterBehavior.Continue;
                else return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
            }), (result) =>
            {
                Core.Log.Debug($"{result} -- {result.VisualHit}");
                Hits.Add(result.VisualHit);
                return HitTestResultBehavior.Continue;
            }, new PointHitTestParameters(point));
            */

            MouseEventArgs args = new MouseEventArgs(e.MouseDevice, e.Timestamp, e.StylusDevice)
            {
                RoutedEvent = e.RoutedEvent,
                Source = this,
            };

            var parent = this.FindParent<FrameworkElement>();

            if (parent == null) return;
            parent.RaiseEvent(args);
            /*
            foreach (object hit in Hits)
            {
                if (hit is UIElement)
                    ((UIElement)hit).RaiseEvent(args);
            }
            */
            e.Handled = true;
        }
        #endregion

        #endregion
    }
}
