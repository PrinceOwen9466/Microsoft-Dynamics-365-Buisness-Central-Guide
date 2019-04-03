using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Interactivity;
using Forms = System.Windows.Forms;

namespace Guide.Common.Infrastructure.Resources.Behaviors
{
    public class MouseBubbleBehavior : Behavior<WindowsFormsHost>
    {
        #region Properties


        #region Event Handlers
        Forms.MouseEventHandler MouseMoveHandler;
        Forms.MouseEventHandler MouseUpHandler;
        Forms.MouseEventHandler MouseDownHandler;
        #endregion

        #endregion


        #region Constructors
        public MouseBubbleBehavior()
        {
            /*
             * *****    Winforms Point to WPF   ****
                Point point = new Point(e.X, e.Y);
                point = PresentationSource.FromVisual(AssociatedObject).CompositionTarget
                    .TransformFromDevice.Transform(point);
            */
            MouseMoveHandler = (s, e) =>
            {
                Core.Log.Debug($"({e.X}, {e.Y})");
                Point point = new Point(e.X, e.Y);
                point = PresentationSource.FromVisual(AssociatedObject).CompositionTarget
                    .TransformFromDevice.Transform(point);
                Core.Log.Debug($"WPF ({point.X}, {point.Y})");
                AssociatedObject.RaiseEvent(new MouseEventArgs(Mouse.PrimaryDevice, 0)
                {
                    Source = AssociatedObject,
                    RoutedEvent = UIElement.MouseMoveEvent
                });
            };

            MouseUpHandler = (s, e) =>
            {
                var button = ConvertToWpf(e.Button);
                if (!button.HasValue) return;

                AssociatedObject.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, button.Value)
                {
                    Source = AssociatedObject,
                    RoutedEvent = UIElement.MouseUpEvent
                });
            };

            MouseDownHandler = (s, e) =>
            {
                var button = ConvertToWpf(e.Button);
                if (!button.HasValue) return;

                AssociatedObject.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, button.Value)
                {
                    Source = AssociatedObject,
                    RoutedEvent = UIElement.MouseDownEvent
                });
            };
        }
        #endregion


        #region Methods

        #region Overries
        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject.Child != null)
            {
                AssociatedObject.Child.MouseMove += MouseMoveHandler;
                AssociatedObject.Child.MouseUp += MouseUpHandler;
                AssociatedObject.Child.MouseDown += MouseDownHandler;
            }

            AssociatedObject.ChildChanged += (s, e) =>
            {
                var prev = e.PreviousChild as Forms.Control;
                var child = AssociatedObject.Child;

                if (prev != null)
                {
                    prev.MouseMove -= MouseMoveHandler;
                    prev.MouseUp -= MouseUpHandler;
                    prev.MouseDown -= MouseDownHandler;
                }

                if (child == null) return;

                
                child.MouseMove += (se, ev) =>
                {
                    Core.Log.Debug($"({ev.X}, {ev.Y})");
                    Point point = new Point(ev.X, ev.Y);
                    point = PresentationSource.FromVisual(AssociatedObject).CompositionTarget
                        .TransformFromDevice.Transform(point);
                    Core.Log.Debug($"WPF ({point.X}, {point.Y})");
                };

                child.MouseUp += MouseUpHandler;
                child.MouseDown += MouseDownHandler;
            };

        }
        #endregion

        MouseButton? ConvertToWpf(Forms.MouseButtons button)
        {
            switch (button)
            {
                case Forms.MouseButtons.Left: return MouseButton.Left;
                case Forms.MouseButtons.None: return null;
                case Forms.MouseButtons.Right: return MouseButton.Right;
                case Forms.MouseButtons.Middle: return MouseButton.Middle;
                case Forms.MouseButtons.XButton1: return MouseButton.XButton1;
                case Forms.MouseButtons.XButton2: return MouseButton.XButton2;
                default: throw new ArgumentOutOfRangeException($"Unrecognized button: {button}");
            }
        }

        #endregion
    }
}
