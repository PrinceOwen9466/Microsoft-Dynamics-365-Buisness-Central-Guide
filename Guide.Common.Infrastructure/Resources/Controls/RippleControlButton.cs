using Guide.Common.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    public class RippleControlButton : Button
    {
        #region Properties

        #region Dependency Properties
        public object TransitionContent
        {
            get { return (object)GetValue(TransitionContentProperty); }
            set { SetValue(TransitionContentProperty, value); }
        }

        public static readonly DependencyProperty TransitionContentProperty =
            DependencyProperty.Register("TransitionContent", typeof(object), typeof(RippleControlButton), new UIPropertyMetadata(null));
        #endregion

        #endregion

        #region Constructors
        public RippleControlButton()
        {
            
        }
        #endregion

        #region Methods

        protected override void OnClick()
        {
            RippleControl control = this.FindParent<RippleControl>();
            if (control == null) return;

            Point point = System.Windows.Input.Mouse.GetPosition(control);
            control.StartPointX = point.X;
            control.StartPointY = point.Y;

            control.TransitionBrush = this.Background;
            control.TransitionDuration = TimeSpan.FromSeconds(.5);
            control.ExtraContent = TransitionContent;
            control.StartTransition();
        }

        public void Activate()
        {
            RippleControl control = this.FindParent<RippleControl>();
            if (control == null) return;
            Rect dimensions = this.GetAbsolutePlacement();
            control.StartPointX = dimensions.X;
            control.StartPointY = dimensions.Y;

            control.TransitionBrush = this.Background;
            control.TransitionDuration = TimeSpan.FromSeconds(.5);
            control.ExtraContent = TransitionContent;
            control.StartTransition();
        }
        #endregion
    }
}
