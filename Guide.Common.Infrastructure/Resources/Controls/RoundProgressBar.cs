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
    public class RoundProgressBar : ProgressBar
    {
        #region Properties

        #region Dependency Properties
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(RoundProgressBar), new UIPropertyMetadata(50.0));



        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(RoundProgressBar), new PropertyMetadata(0.0));

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(RoundProgressBar), new PropertyMetadata(Brushes.Transparent));



        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(RoundProgressBar), new PropertyMetadata(Brushes.Transparent));





        #endregion

        #endregion

        #region Constructors
        static RoundProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RoundProgressBar), new FrameworkPropertyMetadata(typeof(RoundProgressBar)));
        }
        #endregion

        #region Methods


        #endregion
    }
}
