using Guide.Common.Infrastructure.Resources.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    public class SvgImage : Image
    {
        #region Properties

        #region Dependency Properties
        public SvgSource SvgSource
        {
            get { return (SvgSource)GetValue(SvgSourceProperty); }
            set { SetValue(SvgSourceProperty, value); }
        }

        public static readonly DependencyProperty SvgSourceProperty =
            DependencyProperty.Register("SvgSource", typeof(SvgSource), typeof(SvgImage), new UIPropertyMetadata(null, (s, e) =>
            {
                if (!(s is SvgImage) || !(e.NewValue is SvgSource)) return;

                var svg = (SvgImage)s;

                if (!(svg.Source is DrawingImage))
                    svg.Source = new DrawingImage();

                if (!(e.NewValue is SvgSource)) return;
                SvgSource svgSource = ((SvgSource)e.NewValue);

                if (!svgSource.IsActive) svgSource.Activate();
                if (!svgSource.IsActive) return;

                svg.DrawingSource = ((SvgSource)e.NewValue).Drawing;
                ((DrawingImage)svg.Source).Drawing = svg.DrawingSource;
            }));

        public DrawingGroup DrawingSource
        {
            get { return (DrawingGroup)GetValue(DrawingSourceProperty); }
            private set { SetValue(DrawingSourceProperty, value); }
        }

        public static readonly DependencyProperty DrawingSourceProperty =
            DependencyProperty.Register("DrawingSource", typeof(DrawingGroup), typeof(SvgImage), new PropertyMetadata(null));
        #endregion

        #endregion
    }
}
