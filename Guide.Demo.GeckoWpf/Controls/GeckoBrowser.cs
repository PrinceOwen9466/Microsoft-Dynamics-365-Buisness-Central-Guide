using Gecko;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

namespace Guide.Demo.GeckoWpf.Controls
{
    public class GeckoBrowser : Control
    {
        #region Properties

        #region Dependency Properties


        public Uri Address
        {
            get { return (Uri)GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }

        public static readonly DependencyProperty AddressProperty =
            DependencyProperty.Register("Address", typeof(Uri), typeof(GeckoBrowser), new UIPropertyMetadata(null, (s, e) =>
            {
                if (!(s is GeckoBrowser)) return;

                Uri uri = (Uri)e.NewValue;
                if (uri == null) return;
                var browser = (GeckoBrowser)s;

                if (browser.Browser == null) return;

                browser.Browser.LoadHtml(File.ReadAllText(uri.OriginalString));
            }));


        #endregion

        #region Internals
        WindowsFormsHost Host { get; set; }
        GeckoWebBrowser Browser { get; set; }
        #endregion

        #endregion

        #region Constructors
        public GeckoBrowser() { }
        #endregion

        #region Methods

        #region Overrides
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Host = GetTemplateChild("PART_HOST") as WindowsFormsHost;
            Browser = new GeckoWebBrowser();
            Host.Child = Browser;

            if (Address != null)
            {
                Browser.LoadHtml(File.ReadAllText(Address.OriginalString));
            }
        }
        #endregion


        #endregion
    }
}
