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
using System.Windows.Input;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    public class GeckoBrowser : Control
    {
        #region Properties

        #region Dependency Properties

        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(GeckoBrowser), new UIPropertyMetadata(null, (s, e) =>
            {
                if (!(s is GeckoBrowser)) return;

                Uri uri = (Uri)e.NewValue;
                if (uri == null) return;
                var browser = (GeckoBrowser)s;

                if (browser.Browser == null) return;

                browser.Browser.Navigate(uri.OriginalString);
            }));


        #endregion

        #region Internals
        Grid MainGrid { get; set; }
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

            MainGrid = GetTemplateChild("PART_GRID") as Grid;
            Host = GetTemplateChild("PART_HOST") as WindowsFormsHost;
            Browser = new GeckoWebBrowser();

            Browser.DomKeyDown += (s, e) =>
            {
                switch (e.KeyCode)
                {
                    case 37:
                        Application.Current.Dispatcher.Invoke(() => MainGrid.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(MainGrid), 0, Key.Left) { RoutedEvent = Keyboard.KeyDownEvent }) );
                        break;

                    case 39:
                        Application.Current.Dispatcher.Invoke(() => MainGrid.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(MainGrid), 0, Key.Right) { RoutedEvent = Keyboard.KeyDownEvent }));
                        break;
                }
                Core.Log.Debug($"Dom Key Down {e.KeyCode}");
            };

            if (Host != null)
                Host.Child = Browser;


            if (Source != null)
            {
                try
                {
                    using (var reader = new StreamReader(Application.GetResourceStream(Source).Stream))
                    {
                        var content = reader.ReadToEnd();
                        Browser.LoadHtml(content);
                    }
                }
                catch (Exception ex)
                {
                    Core.Log.Error(ex);
                }
            }
        }
        #endregion


        #endregion
    }
}
