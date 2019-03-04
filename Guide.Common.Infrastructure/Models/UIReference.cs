using Guide.Common.Infrastructure.Models.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Guide.Common.Infrastructure.Models
{
    public class UIReference : Button, IReference
    {
        #region Properties

        #region Dependency Properties

        public string TargetPageName { get; set; }

        public Section TargetSection
        {
            get { return (Section)GetValue(TargetSectionProperty); }
            set { SetValue(TargetSectionProperty, value); }
        }
        public static readonly DependencyProperty TargetSectionProperty =
            DependencyProperty.Register("TargetSection", typeof(Section), typeof(UIReference), new UIPropertyMetadata(null));



        public Type TargetPage
        {
            get { return (Type)GetValue(TargetPageProperty); }
            set { SetValue(TargetPageProperty, value); }
        }

        public static readonly DependencyProperty TargetPageProperty =
            DependencyProperty.Register("TargetPage", typeof(Type), typeof(UIReference), new UIPropertyMetadata(null));




        public int TargetPageIndex
        {
            get { return (int)GetValue(TargetPageIndexProperty); }
            set { SetValue(TargetPageIndexProperty, value); }
        }

        public static readonly DependencyProperty TargetPageIndexProperty =
            DependencyProperty.Register("TargetPageIndex", typeof(int), typeof(UIReference), new UIPropertyMetadata(0));




        #endregion

        #endregion

        #region Constructors

        public UIReference() { }
        public UIReference(Section section, Type page)
        {
            TargetSection = section;
            TargetPage = page;
        }

        #endregion
    }
}
