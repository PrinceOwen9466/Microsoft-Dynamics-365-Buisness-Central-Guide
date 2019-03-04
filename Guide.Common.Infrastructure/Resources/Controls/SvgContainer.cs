using Containers = Guide.Common.Infrastructure.Resources.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    public class SvgContainer : Image
    {
        #region Properties

        #region Dependency Properties
        public Containers.Svg SvgSource
        {
            get { return (Containers.Svg)GetValue(SvgSourceProperty); }
            set { SetValue(SvgSourceProperty, value); }
        }

        public static readonly DependencyProperty SvgSourceProperty =
            DependencyProperty.Register("SvgSource", typeof(Containers.Svg), typeof(SvgContainer), new PropertyMetadata(null, (s, e) =>
            {
                if (!(s is SvgContainer) || !(e.NewValue is Containers.Svg)) return;

                var container = (SvgContainer)s;
                var svg = (Containers.Svg)e.NewValue;

                if (!svg.IsActive) svg.Activate();
                if (!svg.IsActive) return;

                container.Source = svg.ImageSource;
            }));


        #endregion

        #endregion
    }
}
