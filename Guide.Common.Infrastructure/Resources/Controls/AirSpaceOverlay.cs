using Guide.Common.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    
    public class AirSpaceOverlay : Decorator
    {
        #region Properties

        #region Dependency Properties
        public object OverlayChild
        {
            get => WindowHost.Content;
            set => OverlayContainer.Content = value;
        }



        public AirSpaceOverlayContainer OverlayContainer
        {
            get { return (AirSpaceOverlayContainer)GetValue(OverlayContainerProperty); }
            private set { SetValue(OverlayContainerProperty, value); }
        }

        public static readonly DependencyProperty OverlayContainerProperty =
            DependencyProperty.Register("OverlayContainer", typeof(AirSpaceOverlayContainer), typeof(AirSpaceOverlay), new UIPropertyMetadata(null));


        public UIElement ParentControl
        {
            get { return (UIElement)GetValue(ParentControlProperty); }
            private set { SetValue(ParentControlProperty, value); }
        }
        public static readonly DependencyProperty ParentControlProperty =
            DependencyProperty.Register("ParentControl", typeof(UIElement), typeof(AirSpaceOverlay), new UIPropertyMetadata(null));


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
            OverlayContainer = new AirSpaceOverlayContainer(this);
            WindowHost = new Window()
            {
                Background = null,
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false,
                Focusable = false, 
                Content = OverlayContainer,
                Owner = Application.Current.MainWindow,
            };
            
            IsActive = WindowHost != null;
            if (IsActive)
            {
                WindowHost.PreviewMouseDown += (s, e) => { if (ParentWindow != null) ParentWindow.Focus(); };
                Loaded += (s, e) =>
                {
                    ParentControl = this.FindParent<UserControl>(true);
                };
                /*
                IsVisibleChanged += (s, e) =>
                {
                    if (!IsVisible) WindowHost.Close();
                };
                */
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
            this.FindParent<Panel>().SizeChanged += (s, e) => UpdateOverlaySize();
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

    public class AirSpaceOverlayContainer: ContentControl
    {
        #region Properties

        #region Dependency Properties
        public AirSpaceOverlay OverlayParent
        {
            get { return (AirSpaceOverlay)GetValue(OverlayParentProperty); }
            set { SetValue(OverlayParentProperty, value); }
        }

        public static readonly DependencyProperty OverlayParentProperty =
            DependencyProperty.Register("OverlayParent", typeof(AirSpaceOverlay), typeof(AirSpaceOverlayContainer), new PropertyMetadata(null));
        #endregion

        #endregion

        #region Constructors
        public AirSpaceOverlayContainer() { }
        public AirSpaceOverlayContainer(AirSpaceOverlay overlayParent)
        {
            OverlayParent = overlayParent;
        }
        #endregion
    }
}
