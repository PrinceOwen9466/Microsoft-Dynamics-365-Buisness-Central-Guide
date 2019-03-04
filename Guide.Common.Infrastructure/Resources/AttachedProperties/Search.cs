using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Guide.Common.Infrastructure.Resources.AttachedProperties
{
    public static class Search
    {
        #region Properties

        #region Attached Properties
        public static bool GetSearchActive(DependencyObject obj)
        {
            return (bool)obj.GetValue(SearchActiveProperty);
        }

        public static void SetSearchActive(DependencyObject obj, bool value)
        {
            obj.SetValue(SearchActiveProperty, value);
        }

        public static readonly DependencyProperty SearchActiveProperty =
            DependencyProperty.RegisterAttached("SearchActive", typeof(bool), typeof(Search), new UIPropertyMetadata(false, (s, e) =>
            {
                Core.Log.Debug($"Search Active {s}");
            }));
        #endregion

        #endregion

        #region Methods

        

        #endregion

    }
}
