using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    public class ReflectablePopup : Popup
    {
        #region Properties

        #region Event Handlers
        MouseEventHandler MouseEventHandler;
        #endregion

        #endregion

        #region Constructors
        public ReflectablePopup()
        {
            MouseEventHandler = (s, e) => UpdateOffset();
        }
        #endregion

        #region Methods

        #region Overrides
        protected override void OnOpened(EventArgs e)
        {
            if (PlacementTarget == null) return;
            Application.Current.MainWindow.MouseMove += MouseEventHandler;
            UpdateOffset();
            base.OnOpened(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (PlacementTarget == null) return;
            Application.Current.MainWindow.MouseMove -= MouseEventHandler;
            base.OnClosed(e);
        }
        #endregion

        void UpdateOffset()
        {
            if (PlacementTarget == null) return;
            Point point = Mouse.GetPosition(PlacementTarget);
            double difference = Child is FrameworkElement ? ((FrameworkElement)Child).ActualWidth / 2.0 : .0;
            HorizontalOffset = point.X - difference;
        }

        #endregion
    }
}
