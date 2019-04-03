using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Guide.Common.Infrastructure.Resources.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        #region Properties
        public Visibility HiddenType { get; set; } = Visibility.Hidden;
        public bool Inverse { get; set; }
        #endregion

        #region Methods
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = HiddenType;
            if (value is string)
                visibility = string.IsNullOrWhiteSpace((string)value) ? HiddenType : Visibility.Visible;
            else visibility = null == value ? HiddenType : Visibility.Visible;


            // Core.Log.Info($"'{value}'  ---- Inverse: {Inverse} ----- First Result: {visibility}");
            
            if (Inverse)
                visibility = visibility == Visibility.Visible ? HiddenType : Visibility.Visible;

            // Core.Log.Warn($"'{value}' ---- Inverse: {Inverse} ----- Result: {visibility}");

            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
