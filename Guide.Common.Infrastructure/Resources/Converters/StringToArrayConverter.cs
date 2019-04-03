﻿using Guide.Common.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Guide.Common.Infrastructure.Resources.Converters
{
    public class StringToArrayConverter : TypeConverter
    {
        #region Methods

        #region Overrides
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string s = value as string;
            if (s != null) return s.Split(',');

            return base.ConvertFrom(context, culture, value);
        }
        #endregion

        #endregion
    }
}