using Gecko;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Guide.Demos.GeckoFX
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Xpcom.Initialize("Firefox");
            GeckoWebBrowser browser = new GeckoWebBrowser();
            this.Controls.Add(browser);
            browser.Navigate("www.bbc.com");

        }
    }
}
