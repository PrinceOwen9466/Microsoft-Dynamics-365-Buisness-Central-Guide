using Guide.Highlighter.Extensions;
using Guide.Highlighter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Guide.Highlighter.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell : Window
    {
        #region Properties

        #endregion

        #region Constructors
        public Shell()
        {
#if DEBUG
            ConsoleManager.Show();
#endif

            InitializeComponent();
            DataContext = new ShellViewModel();
        }
        #endregion

        #region Methods

        #region Overrides
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            IntPtr hWnd = new WindowInteropHelper(this).Handle;
            Win32.SetWindowExTransparent(hWnd);

            
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            //MessageBox.Show("Moved");
        }
        #endregion

        #endregion

    }
}
