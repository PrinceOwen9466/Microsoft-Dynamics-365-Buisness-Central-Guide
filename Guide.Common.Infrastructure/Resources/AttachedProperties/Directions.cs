using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Guide.Common.Infrastructure.Resources.AttachedProperties
{
    public static class Directions
    {
        #region Properties

        #region Attached Properties


        public static double GetHorizontalDirection(DependencyObject obj)
        {
            return (double)obj.GetValue(HorizontalDirectionProperty);
        }

        public static void SetHorizontalDirection(DependencyObject obj, double value)
        {
            obj.SetValue(HorizontalDirectionProperty, value);
        }
        public static readonly DependencyProperty HorizontalDirectionProperty =
            DependencyProperty.RegisterAttached("HorizontalDirection", typeof(double), typeof(Directions), new UIPropertyMetadata(.0));

        public static double GetVerticalDirection(DependencyObject obj)
        {
            return (double)obj.GetValue(VerticalDirectionProperty);
        }

        public static void SetVerticalDirection(DependencyObject obj, double value)
        {
            obj.SetValue(VerticalDirectionProperty, value);
        }
        public static readonly DependencyProperty VerticalDirectionProperty =
            DependencyProperty.RegisterAttached("VerticalDirection", typeof(double), typeof(Directions), new UIPropertyMetadata(.0));



        #endregion

        #endregion

        #region Constructors


        #endregion
    }
}
