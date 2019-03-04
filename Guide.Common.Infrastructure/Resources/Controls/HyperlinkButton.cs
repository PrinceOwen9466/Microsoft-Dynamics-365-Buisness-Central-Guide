using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    public class HyperlinkButton : Button
    {
        #region Properties


        public Uri NavigateUri
        {
            get { return (Uri)GetValue(NavigateUriProperty); }
            set { SetValue(NavigateUriProperty, value); }
        }

        public static readonly DependencyProperty NavigateUriProperty =
            DependencyProperty.Register("NavigateUri", typeof(Uri), typeof(HyperlinkButton), new UIPropertyMetadata(null));


        #endregion

        #region Constructors
        public HyperlinkButton() { }
        #endregion

        #region Methods
        protected override void OnClick()
        {
            base.OnClick();
            if (NavigateUri == null) return;
            try
            {
                Process.Start(new ProcessStartInfo(NavigateUri.AbsoluteUri));
            }
            catch { }
        }
        #endregion
    }
}
