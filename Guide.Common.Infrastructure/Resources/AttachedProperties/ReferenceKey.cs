using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Guide.Common.Infrastructure.Resources.AttachedProperties
{
    public static class ReferenceKey
    {
        #region Properties

        #region Dependency Properties
        public static string GetKey(DependencyObject obj)
        {
            return (string)obj.GetValue(KeyProperty);
        }

        public static void SetKey(DependencyObject obj, string value)
        {
            obj.SetValue(KeyProperty, value);
        }

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.RegisterAttached("Key", typeof(string), typeof(ReferenceKey), new UIPropertyMetadata(Guid.NewGuid().ToString()));

        #endregion

        #region Internals
        #endregion

        #endregion

        #region Methods

        #region Overrides

        
        #endregion


        #endregion
    }
}
