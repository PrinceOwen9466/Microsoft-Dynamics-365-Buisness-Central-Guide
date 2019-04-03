using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace Guide.Common.Infrastructure.Resources.Controls
{
    public class ActiveHyperlink : Hyperlink
    {
        #region Properties
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(ActiveHyperlink), new UIPropertyMetadata(true));
        #endregion

        #region Constructors
        public ActiveHyperlink()
        {
            RequestNavigate += OnRequestNavigate;
        }

        public ActiveHyperlink(Inline childInline) : base(childInline)
        {
            RequestNavigate += OnRequestNavigate;
        }

        #endregion

        #region Methods

        #region Command Handlers
        private void OnRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (!IsActive) return;
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }
            catch { }
        }
        #endregion

        #endregion
    }
}
