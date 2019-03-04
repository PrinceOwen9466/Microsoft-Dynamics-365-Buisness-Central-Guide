using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Guide.Demos.MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Player.Play();
            /*
            vlcPlayer.MediaPlayer.VlcLibDirectory = new DirectoryInfo(@"c:\Program Files (x86)\VideoLAN\VLC\");
            vlcPlayer.MediaPlayer.EndInit();
            var file = new FileInfo(@"C:\Users\Prince\Videos\Captures\Searching.mp4");
            vlcPlayer.MediaPlayer.Play(file);
            */
        }
    }
}
