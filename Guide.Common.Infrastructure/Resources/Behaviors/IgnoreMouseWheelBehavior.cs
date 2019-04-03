using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Guide.Common.Infrastructure.Resources.Behaviors
{
    public class IgnoreMouseWheelBehavior : Behavior<FrameworkElement>
    {
        #region Properties

        #region Events
        MouseWheelEventHandler MouseWheelEventHandler = null;
        #endregion

        #endregion

        public IgnoreMouseWheelBehavior()
        {
            MouseWheelEventHandler = (s, e) =>
            {
                e.Handled = true;
                MouseWheelEventArgs @event = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) { RoutedEvent = FrameworkElement.MouseWheelEvent };
                AssociatedObject.RaiseEvent(@event);
            };
        }

        #region Methods

        #region Overrides
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseWheel += MouseWheelEventHandler;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseWheel -= MouseWheelEventHandler;
        }
        #endregion

        #endregion
    }
}
