using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Guide.Common.Infrastructure.Resources.Converters
{
    public class TimespanToDoubleConverter : IValueConverter
    {
        #region Methods

        #region IValueConverter Implementation
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan)
                return ((TimeSpan)value).TotalSeconds;
            else if (value is double)
                return TimeSpan.FromSeconds((double)value);
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan)
                return ((TimeSpan)value).TotalSeconds;
            else if (value is double)
                return TimeSpan.FromSeconds((double)value);
            return value;
        }
        #endregion

        #endregion
    }
}
