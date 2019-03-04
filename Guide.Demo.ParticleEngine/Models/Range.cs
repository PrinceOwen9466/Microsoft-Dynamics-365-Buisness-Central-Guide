using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Guide.Demo.ParticleEngine.Models
{
    public class Range : DependencyObject
    {
        #region Properties

        #region Dependency Properties

        public double Start
        {
            get { return (double)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }
        public static readonly DependencyProperty StartProperty =
            DependencyProperty.Register("Start", typeof(double), typeof(Range), new PropertyMetadata(.0));

        public double End
        {
            get { return (double)GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }
        public static readonly DependencyProperty EndProperty =
            DependencyProperty.Register("End", typeof(double), typeof(Range), new PropertyMetadata(.0));


        #endregion

        #endregion
    }
}
