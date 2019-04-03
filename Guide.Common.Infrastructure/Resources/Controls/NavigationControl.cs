using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    public class NavigationControl : Control
    {
        #region Properties

        #region Dependency Properties

        public Button LeftButton
        {
            get { return (Button)GetValue(LeftButtonProperty); }
            private set { SetValue(LeftButtonProperty, value); }
        }

        public static readonly DependencyProperty LeftButtonProperty =
            DependencyProperty.Register("LeftButton", typeof(Button), typeof(NavigationControl), new UIPropertyMetadata(null));


        public Button RightButton
        {
            get { return (Button)GetValue(RightButtonProperty); }
            private set { SetValue(RightButtonProperty, value); }
        }

        public static readonly DependencyProperty RightButtonProperty =
            DependencyProperty.Register("RightButton", typeof(Button), typeof(NavigationControl), new UIPropertyMetadata(null));


        #endregion

        #endregion

        #region Constructors
        static NavigationControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationControl), new FrameworkPropertyMetadata(typeof(NavigationControl)));
        }
        #endregion

        #region Methods

        #region Overrides
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            LeftButton = GetTemplateChild("LeftButton") as Button;
            RightButton = this.GetTemplateChild("RightButton") as Button;
        }
        #endregion

        #endregion
    }
}
