using Guide.Common.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    public class AirSpaceOverlay : Decorator
    {
        #region Properties

        #region Events
        public event RoutedEventHandler ChildClicked;
        #endregion

        #region Dependency Properties
        public object OverlayChild
        {
            get => WindowHost.Content;
            set
            {
                WindowHost.Content = value;

                if (value is Button)
                {
                    var element = (Button)value;
                    element.Click += (s, e) => ChildClicked?.Invoke(this, e);
                }
            }
        }
        #endregion


        #region Internals

        #region Elements
        Window WindowHost { get; set; }
        Window ParentWindow { get; set; }
        bool IsActive { get; set; }
        #endregion

        #endregion


        #endregion

        #region Constructors
        public AirSpaceOverlay()
        {
            WindowHost = new Window()
            {
                Background = Brushes.Transparent,
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false,
                Focusable = false
            };

            IsActive = WindowHost != null;

            if (IsActive)
            {
                WindowHost.PreviewMouseDown += (s, e) => { if (ParentWindow != null) ParentWindow.Focus(); };
                IsVisibleChanged += (s, e) =>
                {
                    if (!IsVisible) WindowHost.Close();
                };
            }
        }
        #endregion


        #region Methods

        #region Overrides
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (!IsActive) return;
            UpdateOverlaySize();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (!IsActive) return;
            if (WindowHost.Visibility == Visibility.Visible) return;

            UpdateOverlaySize();
            WindowHost.Show();
            ParentWindow = this.FindParent<Window>();
            WindowHost.Owner = ParentWindow;
            ParentWindow.LocationChanged += (s, e) => UpdateOverlaySize();
            ParentWindow.SizeChanged += (s, e) => UpdateOverlaySize();
            ParentWindow.Closed += (s, e) => WindowHost.Close();
        }
        #endregion

        void UpdateOverlaySize()
        {
            if (!IsActive) return;
            Point point = PointToScreen(new Point(0, 0));
            WindowHost.Left = point.X;
            WindowHost.Top = point.Y;
            WindowHost.Width = ActualWidth;
            WindowHost.Height = ActualHeight;
        }

        #endregion
    }
}
