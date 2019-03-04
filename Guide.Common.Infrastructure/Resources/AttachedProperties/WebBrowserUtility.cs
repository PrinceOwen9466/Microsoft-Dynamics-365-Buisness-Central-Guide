using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Guide.Common.Infrastructure.Resources.AttachedProperties
{
    public static class WebBrowserUtility
    {
        public static readonly DependencyProperty BindableSourceProperty =
            DependencyProperty.RegisterAttached("BindableSource", typeof(string), typeof(WebBrowserUtility), new UIPropertyMetadata(null, BindableSourcePropertyChanged));

        public static string GetBindableSource(DependencyObject obj)
        {
            return (string)obj.GetValue(BindableSourceProperty);
        }

        public static void SetBindableSource(DependencyObject obj, string value)
        {
            obj.SetValue(BindableSourceProperty, value);
        }

        public static void BindableSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser browser = o as WebBrowser;
            if (browser != null)
            {
                if (!browser.IsLoaded)
                    browser.Loaded += (s, ev) => browser.Navigate((string)e.NewValue);
                else
                browser.Navigate((string)e.NewValue);
                //string uri = e.NewValue as string;
                //browser.Source = !String.IsNullOrEmpty(uri) ? new Uri(uri) : null;
            }
        }

    }
}
